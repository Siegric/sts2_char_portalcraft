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
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Ralmia, Sonic Boom — X cost Skill.
/// Select up to X Artifacts (cost ≤ 2) in your hand. Activate their effects.
/// Upgrade: X+1 instead.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class RalmiaSonicBoom : sts2_char_portalcraftCard
{
    protected override bool HasEnergyCostX => true;

    public RalmiaSonicBoom() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int xValue = ResolveEnergyXValue();
        if (IsUpgraded)
            xValue++;

        if (xValue <= 0) return;

        bool Filter(CardModel c) =>
            c != this &&
            c is ArtifactCard &&
            c.EnergyCost.Canonical <= 2;

        var handArtifacts = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();

        if (handArtifacts.Count > 0)
        {
            int maxSelect = System.Math.Min(xValue, handArtifacts.Count);
            var prefs = new CardSelectorPrefs(
                new LocString("card_selection", "RALMIA_PROMPT"),
                minCount: 0,
                maxCount: maxSelect
            );

            var selected = await CardSelectCmd.FromHand(
                choiceContext, Owner, prefs, Filter, this);

            foreach (var card in selected)
            {
                if (card is ArtifactCard artifact)
                {
                    await artifact.ActivateEffect(choiceContext);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
    }
}
