using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;

/// <summary>
/// Defines all merge recipes for the Artifact system.
/// T0→T1: Fixed recipes (same-type pairs only).
/// T1→T2: Variable discard count — total cost of discarded cards determines result.
/// T2→T3: Fixed recipe (all 3 distinct T2 types required).
/// </summary>
public static class MergeRecipes
{
    public record MergeRecipe(Type PlayedType, Type[] DiscardedTypes, Type ResultType);

    private static readonly List<MergeRecipe> _recipes = new();
    private static bool _initialized;

    public static IReadOnlyList<MergeRecipe> All
    {
        get
        {
            EnsureInitialized();
            return _recipes;
        }
    }

    private static void EnsureInitialized()
    {
        if (_initialized) return;
        _initialized = true;

        // T0 + T0 → T1 (same-type pairs only)
        Add<GearOfAmbition, GearOfAmbition, StrikerArtifact>();
        Add<GearOfRemembrance, GearOfRemembrance, FortifierArtifact>();

        // T1 → T2 is handled dynamically in FindT1MergeResult (variable discard count)
        // T2 → T3 is handled in FindResult (need all 3 distinct T2 types)
    }

    private static void Add<TPlayed, TDiscarded, TResult>()
        where TPlayed : ArtifactCard
        where TDiscarded : ArtifactCard
        where TResult : ArtifactCard
    {
        _recipes.Add(new MergeRecipe(typeof(TPlayed), new[] { typeof(TDiscarded) }, typeof(TResult)));
    }

    /// <summary>
    /// Given the played card and the discarded cards, find the matching recipe result type.
    /// Returns null if no recipe matches.
    /// </summary>
    public static Type? FindResult(CardModel playedCard, IReadOnlyList<CardModel> discardedCards)
    {
        EnsureInitialized();

        if (discardedCards.Count == 0) return null;

        if (playedCard is ArtifactCard pc)
        {
            // T1 → T2: variable discard, result based on total cost
            if (pc.Tier == ArtifactTier.T1_Iron)
            {
                return FindT1MergeResult(discardedCards);
            }

            // T2 → T3: need exactly 2 discarded T2s, all 3 T2 types present
            if (pc.Tier == ArtifactTier.T2_Steel && discardedCards.Count == 2)
            {
                var allTypes = new HashSet<Type> { playedCard.GetType() };
                foreach (var c in discardedCards) allTypes.Add(c.GetType());

                if (allTypes.Contains(typeof(OminousArtifactAlpha)) &&
                    allTypes.Contains(typeof(OminousArtifactBeta)) &&
                    allTypes.Contains(typeof(OminousArtifactGamma)))
                {
                    return typeof(MasterworkOmega);
                }
                return null;
            }
        }

        // Standard single-discard recipes (T0 → T1)
        if (discardedCards.Count != 1) return null;
        var playedType = playedCard.GetType();
        var discardedType = discardedCards[0].GetType();

        return _recipes
            .Where(r => r.PlayedType == playedType && r.DiscardedTypes.Length == 1 && r.DiscardedTypes[0] == discardedType)
            .Select(r => r.ResultType)
            .FirstOrDefault();
    }

    /// <summary>
    /// For T1→T2 merges: determine result based on total energy cost of discarded cards.
    /// 1 → α, 2 → β, 3+ → γ
    /// </summary>
    private static Type? FindT1MergeResult(IReadOnlyList<CardModel> discardedCards)
    {
        int totalCost = 0;
        foreach (var card in discardedCards)
        {
            totalCost += card.EnergyCost.Canonical;
        }

        // If only T0s were discarded (cost 0 each), total = 0 → treat as 1
        if (totalCost <= 0) totalCost = discardedCards.Count;

        return totalCost switch
        {
            1 => typeof(OminousArtifactAlpha),
            2 => typeof(OminousArtifactBeta),
            _ => typeof(OminousArtifactGamma), // 3+
        };
    }

    /// <summary>
    /// Get all card types that are valid discard targets for a given played card type.
    /// </summary>
    public static HashSet<Type> GetValidDiscardTypes(Type playedType, ArtifactTier tier)
    {
        EnsureInitialized();
        var validTypes = new HashSet<Type>();

        if (tier == ArtifactTier.T1_Iron)
        {
            // T1 can discard any other T1
            validTypes.Add(typeof(StrikerArtifact));
            validTypes.Add(typeof(FortifierArtifact));
        }
        else if (tier == ArtifactTier.T2_Steel)
        {
            // For T2, valid discard targets are the other two T2 types
            Type[] allT2 = { typeof(OminousArtifactAlpha), typeof(OminousArtifactBeta), typeof(OminousArtifactGamma) };
            foreach (var t in allT2)
            {
                if (t != playedType) validTypes.Add(t);
            }
        }
        else
        {
            // T0: use recipe-based lookup
            foreach (var recipe in _recipes)
            {
                if (recipe.PlayedType == playedType)
                {
                    foreach (var t in recipe.DiscardedTypes)
                        validTypes.Add(t);
                }
            }
        }

        return validTypes;
    }
}
