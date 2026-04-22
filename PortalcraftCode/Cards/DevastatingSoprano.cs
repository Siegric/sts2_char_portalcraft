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
using sts2_char_portalcraft.PortalcraftCode.Cards.Omen;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class DevastatingSoprano : PortalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<WhitePsalmNewRevelation>(),
    };

    public DevastatingSoprano() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool AnyCard(CardModel c) => c != this;
        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(AnyCard).ToList();
        if (handCards.Count == 0) return;

        var exhaustPrefs = new CardSelectorPrefs(
            new LocString("card_selection", "DEVASTATING_SOPRANO_PROMPT"),
            minCount: 1,
            maxCount: 1
        );

        var toExhaust = (await CardSelectCmd.FromHand(choiceContext, Owner, exhaustPrefs, AnyCard, this)).ToList();
        if (toExhaust.Count == 0) return;

        foreach (var card in toExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        // Add White Psalm to hand
        await WhitePsalmNewRevelation.CreateInHand(Owner, CombatState);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
