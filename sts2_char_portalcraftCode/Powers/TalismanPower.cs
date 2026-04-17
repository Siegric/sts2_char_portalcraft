using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Omen;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

public sealed class TalismanPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private const int BaseBlockAmount = 2;
    private const int BaseDamageAmount = 2;
    private const int WastelandTokenDrawCount = 1;
    
    private bool _isTransforming;

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;

        var player = CombatState.Players.FirstOrDefault(p => p.Creature == Owner);
        if (player == null) return;

        var handCards = PileType.Hand.GetPile(player).Cards.ToList();

        var whitePsalms = handCards.Where(TalismanHelper.IsWhitePsalm).ToList();
        var blackPsalms = handCards.Where(TalismanHelper.IsBlackPsalm).ToList();

        // White Psalm: gain block
        foreach (var psalm in whitePsalms)
        {
            Flash();
            int block = GetWhitePsalmBlock(psalm);
            await CreatureCmd.GainBlock(Owner, block, ValueProp.Unpowered, null);
        }

        // Black Psalm: deal AoE
        foreach (var psalm in blackPsalms)
        {
            Flash();
            int damage = GetBlackPsalmDamage(psalm);
            await DealAoeDamage(choiceContext, damage);
        }
        
        _isTransforming = true;
        try
        {
            foreach (var psalm in whitePsalms)
            {
                await CardCmd.Exhaust(choiceContext, psalm);
                var black = CombatState.CreateCard<BlackPsalmNewRevelation>(player);
                await CardPileCmd.AddGeneratedCardToCombat(black, PileType.Hand, addedByPlayer: true);
            }

            foreach (var psalm in blackPsalms)
            {
                await CardCmd.Exhaust(choiceContext, psalm);
                var white = CombatState.CreateCard<WhitePsalmNewRevelation>(player);
                await CardPileCmd.AddGeneratedCardToCombat(white, PileType.Hand, addedByPlayer: true);
            }
        }
        finally
        {
            _isTransforming = false;
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner?.Creature != Owner) return;
        
        if (_isTransforming) return;

        var player = card.Owner;
        
        if (TalismanHelper.IsWhitePsalm(card))
        {
            Flash();
            int block = GetWhitePsalmBlock(card);
            await CreatureCmd.GainBlock(Owner, block, ValueProp.Unpowered, null);

            var black = CombatState.CreateCard<BlackPsalmNewRevelation>(player);
            await CardPileCmd.AddGeneratedCardToCombat(black, PileType.Hand, addedByPlayer: true);
        }
        else if (TalismanHelper.IsBlackPsalm(card))
        {
            Flash();
            int damage = GetBlackPsalmDamage(card);
            await DealAoeDamage(choiceContext, damage);

            var white = CombatState.CreateCard<WhitePsalmNewRevelation>(player);
            await CardPileCmd.AddGeneratedCardToCombat(white, PileType.Hand, addedByPlayer: true);
        }
        else if (card is WastelandOfDestruction)
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, 1, player);
        }
        else if (TalismanHelper.IsWastelandToken(card))
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, WastelandTokenDrawCount, player);
        }
    }

    private async Task DealAoeDamage(PlayerChoiceContext choiceContext, int damage)
    {
        var enemies = CombatState.HittableEnemies.ToList();
        foreach (var enemy in enemies)
        {
            await CreatureCmd.Damage(choiceContext, enemy, damage, ValueProp.Unpowered, Owner, null);
        }
    }

    private int GetWhitePsalmBlock(CardModel card)
    {
        int baseBlock = (card as WhitePsalmNewRevelation)?.BlockValue ?? BaseBlockAmount;
        return baseBlock + GetBeelzebubBonus();
    }

    private int GetBlackPsalmDamage(CardModel card)
    {
        int baseDamage = (card as BlackPsalmNewRevelation)?.DamageValue ?? BaseDamageAmount;
        return baseDamage + GetBeelzebubBonus();
    }

    private int GetBeelzebubBonus()
    {
        var beelzebub = Owner.GetPower<BeelzebubSupremeKingPower>();
        return beelzebub?.Amount ?? 0;
    }
}
