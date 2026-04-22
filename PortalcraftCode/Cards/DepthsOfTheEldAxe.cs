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

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class DepthsOfTheEldAxe : PortalcraftCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public DepthsOfTheEldAxe() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self, showInCardLibrary: true) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool Filter(CardModel c) => c != this && c.EnergyCost.Canonical >= 2;

        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (handCards.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "DEPTHS_COPY_PROMPT"),
            minCount: 1,
            maxCount: 1);

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;

        var original = selected[0];
        await CardCmd.Exhaust(choiceContext, original);
        var copy = CombatState.CloneCard(original);
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
        copy.EnergyCost.AddThisCombat(-1);
    }

    protected override void OnUpgrade() { }
}
