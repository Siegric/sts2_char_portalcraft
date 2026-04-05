using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Biofabrication — 1 cost Ancient Skill.
/// Select a Construct in your hand. Subtract 1 from its cost.
/// Put 3 copies of that card into your draw pile.
/// Upgrade: cost 0.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class Biofabrication : sts2_char_portalcraftCard
{
    public Biofabrication() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "BIOFABRICATION_PROMPT"),
            minCount: 1,
            maxCount: 1);

        bool Filter(CardModel c) => c != this && c is ArtifactCard;

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;

        var chosen = selected[0];

        // Subtract 1 from the selected card's cost (permanent for this combat)
        chosen.EnergyCost.AddThisCombat(-1);

        // Put 3 copies into draw pile
        for (int i = 0; i < 3; i++)
        {
            var copy = ArtifactHelper.CreateByType(chosen.GetType(), CombatState, Owner);
            copy.EnergyCost.AddThisCombat(-1);
            await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
