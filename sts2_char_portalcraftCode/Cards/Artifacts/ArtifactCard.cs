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
[Pool(typeof(sts2_char_portalcraftCardPool))]
public abstract class ArtifactCard : sts2_char_portalcraftCard
{
    public abstract ArtifactTier Tier { get; }
    private int MergeDiscardMax => Tier switch
    {
        ArtifactTier.T0_Artifact => 1,
        ArtifactTier.T1_Iron => 10, 
        ArtifactTier.T2_Steel => 2,
        _ => 0
    };

    private bool CanMerge => Tier != ArtifactTier.T3_Omega;

    protected ArtifactCard(CardType type, TargetType target)
        : base(0, type, CardRarity.Token, target, showInCardLibrary: true)
    {
    }

    protected ArtifactCard(int energyCost, CardType type, TargetType target)
        : base(energyCost, type, CardRarity.Token, target, showInCardLibrary: true)
    {
    }
    
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
            await OnRawPlay(choiceContext, cardPlay);
        }
    }
    protected abstract Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay);
    public virtual Task ActivateEffect(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    private async Task HandleMergeOrRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
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
            
            var selectedList = selected.Where(Filter).ToList();

            if (selectedList.Count > 0)
            {
                var resultType = MergeRecipes.FindResult(this, selectedList);

                if (resultType != null)
                {
                    foreach (var card in selectedList)
                    {
                        await CardCmd.Exhaust(choiceContext, card);
                    }
                    
                    var canonicalCard = (CardModel)typeof(ModelDb)
                        .GetMethod(nameof(ModelDb.Card), System.Type.EmptyTypes)!
                        .MakeGenericMethod(resultType)
                        .Invoke(null, null)!;
                    var resultCard = CombatState.CreateCard(canonicalCard, Owner);
                    await CardPileCmd.AddGeneratedCardToCombat(resultCard, PileType.Hand, addedByPlayer: true);
                    
                    if (CanonicalEnergyCost > 0)
                    {
                        await PlayerCmd.GainEnergy(CanonicalEnergyCost, Owner);
                    }
                    
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
            }
        }
        catch (System.Exception)
        {
        }
        await OnRawPlay(choiceContext, cardPlay);
    }
}
