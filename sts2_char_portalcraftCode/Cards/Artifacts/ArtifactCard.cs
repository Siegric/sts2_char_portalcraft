using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;

/// <summary>
/// Base class for all Artifact cards. Handles the merge-or-play-raw flow:
///   1. Energy is deducted by the card system when played (T0=0, T1=1, T2=2, T3=3).
///   2. If mergeable, prompt player to optionally select discard targets.
///   3. If valid merge targets selected → consume discards, add merged result to hand, refund energy.
///   4. If nothing selected → resolve the raw effect (energy already spent).
///
/// T0→T1: Discard 1 matching T0 (same-type pairs only).
/// T1→T2: Discard 1-N T1s. Total cost of discarded cards determines T2 variant (α/β/γ).
/// T2→T3: Discard the other 2 distinct T2 types.
/// T3 has nothing to merge into and just plays raw.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public abstract class ArtifactCard : sts2_char_portalcraftCard
{
    public abstract ArtifactTier Tier { get; }

    /// <summary>Max number of cards to discard for merging.</summary>
    private int MergeDiscardMax => Tier switch
    {
        ArtifactTier.T0_Artifact => 1,
        ArtifactTier.T1_Iron => 10, // Variable: select as many T1s as desired
        ArtifactTier.T2_Steel => 2,
        _ => 0
    };

    private bool CanMerge => Tier != ArtifactTier.T3_Omega;

    protected ArtifactCard(CardType type, TargetType target)
        : base(0, type, CardRarity.Token, target)
    {
    }

    protected ArtifactCard(int energyCost, CardType type, TargetType target)
        : base(energyCost, type, CardRarity.Token, target)
    {
    }

    // All artifacts have Retain + Exhaust + Artifact tag. T0–T2 also show Fuse.
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        CanMerge
            ? new[] { CardKeyword.Retain, CardKeyword.Exhaust, FuseKeyword.Fuse }
            : new[] { CardKeyword.Retain, CardKeyword.Exhaust };

    protected override HashSet<CardTag> CanonicalTags => new() { ArtifactTag.Artifact };

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CanMerge)
        {
            await HandleMergeOrRawPlay(choiceContext, cardPlay);
        }
        else
        {
            // T3 Omega: just play the raw effect (energy already deducted by the card system)
            await OnRawPlay(choiceContext, cardPlay);
        }
    }

    /// <summary>Override this to define the card's raw play effect (damage, block, draw, etc.).</summary>
    protected abstract Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay);

    /// <summary>
    /// Activates this artifact's raw effect without playing it from hand.
    /// Used by cards like Ralmia that trigger artifact effects in-place.
    /// Attack artifacts auto-target (random enemy / lowest HP).
    /// </summary>
    public virtual Task ActivateEffect(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    private async Task HandleMergeOrRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // T2 (Ominous Artifacts): require all 3 distinct T2 types in hand to fuse.
        // If the other two aren't both present, skip straight to raw play.
        if (Tier == ArtifactTier.T2_Steel)
        {
            var handCards = PileType.Hand.GetPile(Owner).Cards;
            var requiredTypes = MergeRecipes.GetValidDiscardTypes(GetType(), Tier);
            if (!requiredTypes.All(rt => handCards.Any(c => c != this && c.GetType() == rt)))
            {
                await OnRawPlay(choiceContext, cardPlay);
                return;
            }
        }

        var validDiscardTypes = MergeRecipes.GetValidDiscardTypes(GetType(), Tier);

        // Filter hand for valid merge targets (must be Artifact cards of valid types, excluding self)
        bool Filter(CardModel c) =>
            c != this &&
            c is ArtifactCard &&
            validDiscardTypes.Contains(c.GetType());

        try
        {
            var handArtifacts = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();

            IEnumerable<CardModel> selected;

            if (handArtifacts.Count > 0)
            {
                // Show selection UI: min 0 (can skip), max = MergeDiscardMax
                int maxSelect = Math.Min(MergeDiscardMax, handArtifacts.Count);
                var prefs = new CardSelectorPrefs(
                    new LocString("card_selection", "ARTIFACT_MERGE_PROMPT"),
                    minCount: 0,
                    maxCount: maxSelect
                );

                selected = await CardSelectCmd.FromHand(
                    choiceContext, Owner, prefs, Filter, this);
            }
            else
            {
                selected = Enumerable.Empty<CardModel>();
            }

            // Re-validate selection against the filter to guard against race conditions (fast clicking)
            var selectedList = selected.Where(Filter).ToList();

            if (selectedList.Count > 0)
            {
                // Attempt merge
                var resultType = MergeRecipes.FindResult(this, selectedList);

                if (resultType != null)
                {
                    // Exhaust the discarded cards
                    foreach (var card in selectedList)
                    {
                        await CardCmd.Exhaust(choiceContext, card);
                    }

                    // Create the merged result and add to hand
                    var canonicalCard = (CardModel)typeof(ModelDb)
                        .GetMethod(nameof(ModelDb.Card), System.Type.EmptyTypes)!
                        .MakeGenericMethod(resultType)
                        .Invoke(null, null)!;
                    var resultCard = CombatState.CreateCard(canonicalCard, Owner);
                    await CardPileCmd.AddGeneratedCardToCombat(resultCard, PileType.Hand, addedByPlayer: true);

                    // Refund the energy cost since merging is free
                    if (CanonicalEnergyCost > 0)
                    {
                        await PlayerCmd.GainEnergy(CanonicalEnergyCost, Owner);
                    }

                    // Notify merge-reactive powers and relics
                    var resultTier = (resultCard as ArtifactCard)?.Tier ?? ArtifactTier.T3_Omega;
                    foreach (var power in Owner.Creature.Powers)
                    {
                        if (power is IMergeListener listener)
                        {
                            await listener.OnArtifactMerged(choiceContext, this, selectedList, resultCard, resultTier);
                        }
                    }
                    foreach (var relic in Owner.Relics)
                    {
                        if (relic is IMergeListener relicListener)
                        {
                            await relicListener.OnArtifactMerged(choiceContext, this, selectedList, resultCard, resultTier);
                        }
                    }

                    return;
                }
                // No valid recipe matched despite selection — fall through to raw play
            }
        }
        catch (System.Exception)
        {
            // Safety net: if anything goes wrong during merge, fall through to raw play
            // so the card always completes its play action and never gets stuck on screen.
        }

        // Raw play: energy was already deducted by the card system, just resolve the effect.
        // This also acts as a safety net — the card always completes its play action.
        await OnRawPlay(choiceContext, cardPlay);
    }
}
