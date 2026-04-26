using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

public static class FuseRecipes
{
    public static Type? FindResult(CardModel playedCard, IReadOnlyList<CardModel> discardedCards)
    {
        if (discardedCards.Count == 0) return null;
        if (playedCard is not ArtifactCard pc) return null;

        switch (pc.Tier)
        {
            case ArtifactTier.T0:
                return FindT0FuseResult(pc);

            case ArtifactTier.T1:
                return FindT1FuseResult(discardedCards);

            case ArtifactTier.T2 when discardedCards.Count == 2:
                var allTypes = new HashSet<Type> { playedCard.GetType() };
                foreach (var c in discardedCards) allTypes.Add(c.GetType());
                if (allTypes.Contains(typeof(OminousArtifactAlpha)) &&
                    allTypes.Contains(typeof(OminousArtifactBeta)) &&
                    allTypes.Contains(typeof(OminousArtifactGamma)))
                {
                    return typeof(MasterworkOmega);
                }
                return null;

            default:
                return null;
        }
    }
    
    private static Type? FindT0FuseResult(ArtifactCard played) => played switch
    {
        GearOfAmbition     => typeof(StrikerArtifact),
        GearOfRemembrance  => typeof(FortifierArtifact),
        _                  => null,
    };
    
    private static Type? FindT1FuseResult(IReadOnlyList<CardModel> discardedCards)
    {
        int totalCost = discardedCards.Sum(c => c.EnergyCost.Canonical);
        if (totalCost <= 0) return null;

        return totalCost switch
        {
            1 => typeof(OminousArtifactAlpha),
            2 => typeof(OminousArtifactBeta),
            _ => typeof(OminousArtifactGamma),
        };
    }

#if FALSE
    // Alternative
    private static Type? FindT1FuseResult_Alt(IReadOnlyList<CardModel> discardedCards)
    {
        if (discardedCards.Count == 0) return null;

        var artifacts = discardedCards.OfType<ArtifactCard>().ToList();
        bool allT0 = artifacts.All(c => c.Tier == ArtifactTier.T0);

        return (allT0, artifacts.Count) switch
        {
            (true, 1) => typeof(OminousArtifactAlpha),
            (true, 2) => typeof(OminousArtifactBeta),
            _         => typeof(OminousArtifactGamma),
        };
    }
#endif
    
    public static HashSet<Type> GetValidDiscardTypes(Type playedType, ArtifactTier tier)
    {
        var validTypes = new HashSet<Type>();

        switch (tier)
        {
            case ArtifactTier.T0:
                foreach (var t in ArtifactHelper.T0Types) validTypes.Add(t);
                break;

            case ArtifactTier.T1:
                foreach (var t in ArtifactHelper.T0Types) validTypes.Add(t);
                foreach (var t in ArtifactHelper.T1Types) validTypes.Add(t);
                foreach (var t in ArtifactHelper.T2Types) validTypes.Add(t);
                break;

            case ArtifactTier.T2:
                foreach (var t in ArtifactHelper.T2Types)
                {
                    if (t != playedType) validTypes.Add(t);
                }
                break;
            
        }

        return validTypes;
    }
}
