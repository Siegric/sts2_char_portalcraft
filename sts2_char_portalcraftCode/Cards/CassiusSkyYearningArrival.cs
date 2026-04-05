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
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Cassius, Sky-Yearning Arrival — 2 cost Uncommon Skill.
/// Select one Artifact in your hand. Deal 5X damage to ALL enemies. X = cost of that Artifact.
/// Add a Fortifier Artifact to your hand.
/// Upgrade: Deal 8X instead.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class CassiusSkyYearningArrival : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 5m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<FortifierArtifact>(),
    };

    public CassiusSkyYearningArrival() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool Filter(CardModel c) => c != this && c is ArtifactCard;

        var handArtifacts = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (handArtifacts.Count > 0)
        {
            var prefs = new CardSelectorPrefs(
                new LocString("card_selection", "CASSIUS_PROMPT"),
                minCount: 1,
                maxCount: 1);

            var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
            if (selected.Count > 0)
            {
                int artifactCost = selected[0].EnergyCost.Canonical;
                int multiplier = (int)DynamicVars["MagicNumber"].BaseValue;
                decimal totalDamage = artifactCost * multiplier;

                if (totalDamage > 0)
                {
                    foreach (var enemy in CombatState.HittableEnemies.ToList())
                    {
                        await CreatureCmd.Damage(choiceContext, enemy, totalDamage, ValueProp.Move, Owner.Creature, this);
                    }
                }
            }
        }

        var fortifier = CombatState.CreateCard<FortifierArtifact>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(fortifier, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(3m);
    }
}
