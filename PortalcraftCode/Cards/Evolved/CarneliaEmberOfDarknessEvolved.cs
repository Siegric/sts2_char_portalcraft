using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

// Evolve: Select an Artifact in your hand and give it "Cannot be Exhausted".
// Fires on every OnPlay (evolved/super-evolved) so the effect re-triggers on
// subsequent draws — see LovestruckPuppeteer for the pattern rationale.
public class CarneliaEmberOfDarknessEvolved : CarneliaEmberOfDarkness
{
    public CarneliaEmberOfDarknessEvolved() : this(EvoTier.Evolved) { }
    protected CarneliaEmberOfDarknessEvolved(EvoTier tier) : base(tier) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        bool Filter(CardModel c) => c != this && c is ArtifactCard;
        var artifacts = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (artifacts.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "CARNELIA_PROMPT"),
            minCount: 0,
            maxCount: 1);

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;

        selected[0].AddKeyword(CannotBeExhaustedKeyword.CannotBeExhausted);
    }
}
