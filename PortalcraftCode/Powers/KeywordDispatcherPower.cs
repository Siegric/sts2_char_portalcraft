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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class KeywordDispatcherPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    public int ArtifactsExhaustedCount =>
        (int)(Owner.GetPower<ArtifactsExhaustedPower>()?.Amount ?? 0m);

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;

        if (player.PlayerCombatState != null)
        {
            await EvoRuntime.ResetTurnLockout(player);
        }

        var scanned = new[] { PileType.Hand, PileType.Draw, PileType.Discard }
            .SelectMany(p => p.GetPile(player).Cards)
            .ToList();
        
        var turnStartCards = scanned.OfType<IOnTurnStartCard>().ToList();
        foreach (var card in turnStartCards)
        {
            await card.OnTurnStart(choiceContext);
        }

        var countdownCards = scanned.OfType<ICountdownCard>().Cast<CardModel>().ToList();
        foreach (var card in countdownCards)
        {
            Flash();
            await CountdownHelper.Tick(choiceContext, card);
        }
        
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner?.Creature != Owner) return;
        if (card is not ISkyboundArtCard) return;

        bool regular = card.Keywords.Contains(SkyboundArtKeyword.SkyboundArt)
                       && Owner.HasPower<SkyboundArtPower>();
        bool super = card.Keywords.Contains(SuperSkyboundArtKeyword.SuperSkyboundArt)
                     && Owner.HasPower<SuperSkyboundArtPower>();

        if (regular || super)
        {
            Flash();
            await SkyboundArtRuntime.FireAsAutoPlay(card, choiceContext);
        }
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player) return;
        var player = Owner.Player;
        if (player == null) return;

        var scanned = new[] { PileType.Hand, PileType.Draw, PileType.Discard }
            .SelectMany(p => p.GetPile(player).Cards)
            .OfType<IOnTurnEndCard>()
            .ToList();

        foreach (var card in scanned)
        {
            await card.OnTurnEnd(choiceContext);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner?.Creature != Owner) return;

        if (card is ArtifactCard)
        {
            await PowerCmd.Apply<ArtifactsExhaustedPower>(choiceContext, Owner, 1, Owner, null, silent: true);
        }

        if (card is ILastWordsCard lastWords)
        {
            Flash();
            await lastWords.OnLastWords(choiceContext);
        }

        if (card.Enchantment is ILastWordsEnchantment enchLastWords)
        {
            Flash();
            await enchLastWords.OnLastWords(choiceContext, card);
        }
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (cardSource == null) return 0m;
        if (cardSource.Owner?.Creature != Owner) return 0m;

        // Evolve / super-evolve damage bonuses are baked into each evolved
        // variant's CanonicalVars (e.g. EustaceHowlOfThunderEvolved declares
        // DamageVar(20m) directly). Class identity drives the stat — fully
        // synced across clients.

        if (target != null && cardSource.Keywords.Contains(BaneKeyword.Bane))
        {
            if (target.HasPower<MinionPower>())
            {
                return 9999m;
            }
            return 10m;
        }

        return 0m;
    }
}
