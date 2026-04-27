using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class CrystallizeRuntime
{
    private static readonly HashSet<CardModel> _active = new();

    public static void Mark(CardModel card) => _active.Add(card);

    public static bool TryConsume(CardModel card) => _active.Remove(card);

    public static bool IsActive(CardModel card) => _active.Contains(card);
    
    public static async Task<bool> TryExecute(CardModel card, PlayerChoiceContext choiceContext)
    {
        if (!_active.Remove(card)) return false;
        if (card is not ICrystallizeCard cc) return false;

        var combatState = card.CombatState;
        if (combatState == null) return false;

        var canonicalAmulet = (CardModel)typeof(ModelDb)
            .GetMethod(nameof(ModelDb.Card), Type.EmptyTypes)!
            .MakeGenericMethod(cc.AmuletFormType)
            .Invoke(null, null)!;
        var amulet = combatState.CreateCard(canonicalAmulet, card.Owner);
        await CardPileCmd.AddGeneratedCardToCombat(amulet, PileType.Hand, true);

        await cc.OnAmuletSpawned(choiceContext, amulet);
        return true;
    }
}
