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
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class PuppetShield : PortalcraftCard
{
    public override bool GainsBlock => true;

    public PuppetShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool Filter(CardModel c) =>
            c != this && PuppetHelper.IsPuppet(c);

        var handPuppets = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (handPuppets.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "PUPPET_SHIELD_PROMPT"),
            minCount: 0,
            maxCount: IsUpgraded ? 4 : 3
        );

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;

        foreach (var puppet in selected)
        {
            decimal dmg = puppet.DynamicVars.Damage?.BaseValue ?? 0m;
            if (dmg > 0)
            {
                await CreatureCmd.GainBlock(Owner.Creature, dmg, ValueProp.Move, cardPlay);
            }
            await CardCmd.Exhaust(choiceContext, puppet);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
