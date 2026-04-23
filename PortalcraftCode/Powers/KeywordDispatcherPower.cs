using System;
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
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Patches;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class KeywordDispatcherPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    private int _artifactsExhausted;

    public int ArtifactsExhaustedCount => _artifactsExhausted;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;

        if (player.PlayerCombatState != null)
        {
            EvoRuntime.ResetTurnLockout(player.PlayerCombatState);
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
        
        var handCards = PileType.Hand.GetPile(player).Cards.ToList();
        foreach (var card in handCards)
        {
            if (card is not ISkyboundArtCard skyCard) continue;
            await CheckAndFireSkyboundArt(card, skyCard, choiceContext);
        }
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner?.Creature != Owner) return;
        if (card is not ISkyboundArtCard skyCard) return;
        await CheckAndFireSkyboundArt(card, skyCard, choiceContext);
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

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner?.Creature != Owner) return;
        if (card is not IEvolvableCard) return;

        var tier = EvoRuntime.GetTier(card);
        if (tier == null) return;

        EvoRuntime.ClearTier(card);
        
        var baseType = FindBaseEvolvableType(card.GetType());
        if (baseType == null) return;

        var baseInstance = CreateBaseInstance(baseType, card);
        if (baseInstance == null) return;

        await CardPileCmd.AddGeneratedCardToCombat(baseInstance, PileType.Discard, addedByPlayer: true);
        using (CannotBeExhaustedPatch.BypassScope())
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner?.Creature != Owner) return;

        if (card is ArtifactCard)
        {
            _artifactsExhausted++;
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
        if (!props.IsPoweredAttack()) return 0m;

        if (EvoRuntime.IsSuperEvolved(cardSource)) return EvoRuntime.SuperEvolveDamageBonus;
        if (EvoRuntime.IsEvolved(cardSource)) return EvoRuntime.EvolveDamageBonus;
        return 0m;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource == null) return 0m;
        if (cardSource.Owner?.Creature != Owner) return 0m;
        if (!props.IsPoweredCardOrMonsterMoveBlock()) return 0m;

        if (EvoRuntime.IsSuperEvolved(cardSource)) return EvoRuntime.SuperEvolveBlockBonus;
        if (EvoRuntime.IsEvolved(cardSource)) return EvoRuntime.EvolveBlockBonus;
        return 0m;
    }
    
    private async Task CheckAndFireSkyboundArt(CardModel card, ISkyboundArtCard skyCard, PlayerChoiceContext ctx)
    {
        SkyboundArtHelper.RefreshGauge(card);
        int gauge = SkyboundArtRuntime.CurrentGauge(card);

        if (gauge >= SkyboundArtRuntime.SuperSkyboundArtThreshold &&
            !SkyboundArtRuntime.HasFiredSuperSkyboundArt(card))
        {
            SkyboundArtRuntime.MarkSuperSkyboundArtFired(card);
            Flash();
            await skyCard.OnSuperSkyboundArt(card, ctx);
        }

        if (gauge >= SkyboundArtRuntime.SkyboundArtThreshold &&
            !SkyboundArtRuntime.HasFiredSkyboundArt(card))
        {
            SkyboundArtRuntime.MarkSkyboundArtFired(card);
            Flash();
            await skyCard.OnSkyboundArt(card, ctx);
        }
    }
    
    private static Type? FindBaseEvolvableType(Type cardType)
    {
        var t = cardType;
        while (t.BaseType != null && typeof(IEvolvableCard).IsAssignableFrom(t.BaseType))
        {
            t = t.BaseType;
        }
        return t != cardType ? t : null;
    }

    private static CardModel? CreateBaseInstance(Type baseType, CardModel sourceCard)
    {
        var owner = sourceCard.Owner;
        var combatState = sourceCard.CombatState;
        if (owner == null || combatState == null) return null;

        var modelDbMethod = typeof(ModelDb).GetMethod(nameof(ModelDb.Card), System.Type.EmptyTypes);
        if (modelDbMethod == null) return null;
        if (modelDbMethod.MakeGenericMethod(baseType).Invoke(null, null) is not CardModel canonical) return null;

        var newCard = combatState.CreateCard(canonical, owner);
        if (newCard == null) return null;

        for (int i = 0; i < sourceCard.CurrentUpgradeLevel; i++) newCard.UpgradeInternal();
        return newCard;
    }

}
