using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;

// Super-Evolve: Select an Artifact in hand (cost ≤ 2) and add an exact copy
// to your hand with Ethereal + 0-cost-this-turn. Same ModelDb.Card<T>() +
// CombatState.CreateCard pattern as EvoCmd.CreateTransformedCard.
public class NewAgeCartographerSuperEvolved : NewAgeCartographerEvolved
{
    public NewAgeCartographerSuperEvolved() : base(EvoTier.SuperEvolved) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        bool Filter(CardModel c) => c != this
                                 && c is ArtifactCard
                                 && c.EnergyCost.Canonical <= 2;

        var artifacts = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (artifacts.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "NEW_AGE_CARTOGRAPHER_COPY_PROMPT"),
            minCount: 0,
            maxCount: 1);

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;

        var source = selected[0];
        var copy = CreateCopy(source);
        if (copy == null) return;

        copy.AddKeyword(CardKeyword.Ethereal);
        copy.EnergyCost.SetThisTurnOrUntilPlayed(0, reduceOnly: true);
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
    }

    private CardModel? CreateCopy(CardModel source)
    {
        var modelDbMethod = typeof(ModelDb).GetMethod(nameof(ModelDb.Card), System.Type.EmptyTypes);
        if (modelDbMethod == null) return null;
        if (modelDbMethod.MakeGenericMethod(source.GetType()).Invoke(null, null) is not CardModel canonical) return null;
        var copy = CombatState.CreateCard(canonical, Owner);
        if (copy == null) return null;
        for (int i = 0; i < source.CurrentUpgradeLevel; i++) copy.UpgradeInternal();
        return copy;
    }
}
