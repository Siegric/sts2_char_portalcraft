using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Marionette Master — 1 cost Skill.
/// Add a Puppet and an Enhanced Puppet to your hand.
/// Upgrade: cost 0.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class MarionetteMaster : sts2_char_portalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Puppet>(),
        HoverTipFactory.FromCard<EnhancedPuppet>(),
    };

    public MarionetteMaster() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Puppet.CreateInHand(Owner, 1, CombatState);
        await EnhancedPuppet.CreateInHand(Owner, 1, CombatState);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
