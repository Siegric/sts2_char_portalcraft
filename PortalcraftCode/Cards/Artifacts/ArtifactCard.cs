using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

[Pool(typeof(PortalcraftCardPool))]
public abstract class ArtifactCard : PortalcraftCard, IFuseCard
{
    public int FuseCost => 0;

    public bool HasValidFusionPartnerInHand()
    {
        if (!CanFuse) return false;
        if (Owner == null) return false;

        var handCards = PileType.Hand.GetPile(Owner).Cards;
        var validDiscardTypes = FuseRecipes.GetValidDiscardTypes(GetType(), Tier);
        
        if (Tier == ArtifactTier.T2)
        {
            return validDiscardTypes.All(rt => handCards.Any(c => c != this && c.GetType() == rt));
        }
        
        return handCards.Any(c => c != this && c is ArtifactCard && validDiscardTypes.Contains(c.GetType()));
    }

    public abstract ArtifactTier Tier { get; }

    private int FuseDiscardMax => Tier switch
    {
        ArtifactTier.T0 => 1,      
        ArtifactTier.T1 => 10,     
        ArtifactTier.T2 => 2,     
        _ => 0,                    
    };

    private bool CanFuse => Tier != ArtifactTier.T3;

    protected ArtifactCard(CardType type, TargetType target)
        : base(0, type, CardRarity.Token, target, showInCardLibrary: true)
    {
    }

    protected ArtifactCard(int energyCost, CardType type, TargetType target)
        : base(energyCost, type, CardRarity.Token, target, showInCardLibrary: true)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        CanFuse
            ? new[] { CardKeyword.Retain, CardKeyword.Exhaust, FuseKeyword.Fuse }
            : new[] { CardKeyword.Retain, CardKeyword.Exhaust };

    protected override HashSet<CardTag> CanonicalTags => new() { ArtifactTag.Artifact };

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CanFuse)
        {
            await HandleFuseOrRawPlay(choiceContext, cardPlay);
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

    private async Task HandleFuseOrRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool fused = FuseRuntime.TryConsume(this);
        
        if (!fused && Tier == ArtifactTier.T2)
        {
            var handCards = PileType.Hand.GetPile(Owner).Cards;
            var requiredTypes = FuseRecipes.GetValidDiscardTypes(GetType(), Tier);
            if (!requiredTypes.All(rt => handCards.Any(c => c != this && c.GetType() == rt)))
            {
                await OnRawPlay(choiceContext, cardPlay);
                return;
            }
        }

        var validDiscardTypes = FuseRecipes.GetValidDiscardTypes(GetType(), Tier);
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
                int maxSelect = Math.Min(FuseDiscardMax, handArtifacts.Count);
                var prefs = new CardSelectorPrefs(
                    new LocString("card_selection", "ARTIFACT_FUSE_PROMPT"),
                    minCount: fused ? 1 : 0,
                    maxCount: maxSelect
                ) { RequireManualConfirmation = fused };

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
                var resultType = FuseRecipes.FindResult(this, selectedList);

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

                    int refund = fused ? 0 : EnergyCost.GetResolved();
                    if (refund > 0)
                    {
                        await PlayerCmd.GainEnergy(refund, Owner);
                    }

                    var resultTier = (resultCard as ArtifactCard)?.Tier ?? ArtifactTier.T3;
                    foreach (var power in Owner.Creature.Powers)
                    {
                        if (power is IFuseListener listener)
                        {
                            await listener.OnArtifactFused(choiceContext, this, selectedList, resultCard, resultTier);
                        }
                    }
                    foreach (var relic in Owner.Relics)
                    {
                        if (relic is IFuseListener relicListener)
                        {
                            await relicListener.OnArtifactFused(choiceContext, this, selectedList, resultCard, resultTier);
                        }
                    }

                    return;
                }
            }
        }
        catch (Exception)
        {
        }

        if (fused) return;

        await OnRawPlay(choiceContext, cardPlay);
    }
}
