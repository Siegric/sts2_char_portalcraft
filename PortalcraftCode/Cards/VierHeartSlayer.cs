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
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class VierHeartSlayer : PortalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => System.Array.Empty<DynamicVar>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<DollSlayer>(),
    };
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public VierHeartSlayer() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool Filter(CardModel c) =>
            c != this && PuppetHelper.IsPuppet(c);

        var handPuppets = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (handPuppets.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "VIER_HEART_SLAYER_PROMPT"),
            minCount: 0,
            maxCount: 1
        );

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;
        
        foreach (var puppet in selected)
        {
            decimal puppetDamage = puppet.DynamicVars.Damage?.BaseValue ?? 0m;

            await CardCmd.Exhaust(choiceContext, puppet);
            var dollSlayer = CombatState.CreateCard<DollSlayer>(Owner);

            dollSlayer.DynamicVars.Damage.BaseValue = puppetDamage;

            await CardPileCmd.AddGeneratedCardToCombat(dollSlayer, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
