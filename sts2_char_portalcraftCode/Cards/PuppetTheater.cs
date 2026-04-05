using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Puppet Theater — 1 cost Power.
/// At the start of each turn, add 1 Puppet to your hand.
/// Upgrade: 2 Puppets instead.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class PuppetTheater : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 1m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<PuppetTheaterPower>(),
        HoverTipFactory.FromCard<Puppets.Puppet>(),
    };

    public PuppetTheater() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int amount = (int)DynamicVars["MagicNumber"].BaseValue;
        await PowerCmd.Apply<PuppetTheaterPower>(Owner.Creature, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(1m);
    }
}
