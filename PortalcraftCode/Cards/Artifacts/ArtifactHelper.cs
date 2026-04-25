using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

public static class ArtifactHelper
{
    public static readonly Type[] T0Types =
        { typeof(GearOfAmbition), typeof(GearOfRemembrance) };

    public static readonly Type[] T1Types =
        { typeof(StrikerArtifact), typeof(FortifierArtifact) };

    public static readonly Type[] T2Types =
        { typeof(OminousArtifactAlpha), typeof(OminousArtifactBeta), typeof(OminousArtifactGamma) };

    public static CardModel CreateByType(Type type, ICombatState combatState, Player owner)
    {
        var canonicalCard = (CardModel)typeof(ModelDb)
            .GetMethod(nameof(ModelDb.Card), System.Type.EmptyTypes)!
            .MakeGenericMethod(type)
            .Invoke(null, null)!;
        return combatState.CreateCard(canonicalCard, owner);
    }

    public static async Task AddRandomArtifacts(Type[] pool, int count, ICombatState combatState, Player owner, Rng rng)
    {
        for (int i = 0; i < count; i++)
        {
            var type = rng.NextItem(pool);
            var card = CreateByType(type, combatState, owner);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, owner);
        }
    }

    public static int CountArtifactsInHand(Player owner)
    {
        int count = 0;
        foreach (var card in PileType.Hand.GetPile(owner).Cards)
        {
            if (card is ArtifactCard) count++;
        }
        return count;
    }
}
