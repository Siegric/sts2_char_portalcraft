using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class SummonHelper
{
    // Tracks card instances that were added to hand via a Summon call.
    // Cards that self-summon on play can check this to avoid infinite recursion
    // (the summoned copy won't produce another summon).
    private static readonly ConditionalWeakTable<CardModel, object> _summoned = new();

    public static bool IsSummoned(CardModel card) => _summoned.TryGetValue(card, out _);

    public static async Task Summon(CardModel card, Player owner)
    {
        if (!_summoned.TryGetValue(card, out _)) _summoned.Add(card, new object());
        card.AddKeyword(CardKeyword.Ethereal);
        if (!card.Keywords.Contains(CardKeyword.Exhaust))
        {
            card.AddKeyword(CardKeyword.Exhaust);
        }
        card.EnergyCost.SetThisTurnOrUntilPlayed(0, reduceOnly: true);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
    }
    
    public static async Task Summon<T>(Player owner, CombatState combatState) where T : CardModel
    {
        var card = combatState.CreateCard<T>(owner);
        await Summon(card, owner);
    }
    
    public static async Task SummonCopyOf(CardModel source, Player owner)
    {
        var combatState = source.CombatState;
        if (combatState == null) return;

        var modelDbMethod = typeof(ModelDb).GetMethod(nameof(ModelDb.Card), System.Type.EmptyTypes);
        if (modelDbMethod == null) return;
        if (modelDbMethod.MakeGenericMethod(source.GetType()).Invoke(null, null) is not CardModel canonical) return;

        var copy = combatState.CreateCard(canonical, owner);
        if (copy == null) return;

        for (int i = 0; i < source.CurrentUpgradeLevel; i++) copy.UpgradeInternal();
        await Summon(copy, owner);
    }
}
