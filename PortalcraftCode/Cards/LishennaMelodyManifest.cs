using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using sts2_char_portalcraft.PortalcraftCode.Cards.Omen;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class LishennaMelodyManifest : PortalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<WhitePsalmNewRevelation>(),
        HoverTipFactory.FromCard<BlackPsalmNewRevelation>(),
    };

    public LishennaMelodyManifest() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await WhitePsalmNewRevelation.CreateInHand(Owner, CombatState);
        await WhitePsalmNewRevelation.CreateInHand(Owner, CombatState);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
