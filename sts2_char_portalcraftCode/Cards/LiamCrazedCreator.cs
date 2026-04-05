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
/// Liam, Crazed Creator — 2 cost Power.
/// Whenever you play a Puppet, gain 2 block and deal 2 damage to a random enemy.
/// For every 5 Puppets played, gain 1 energy.
/// Upgrade: 4 block and 4 damage instead.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class LiamCrazedCreator : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 2m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<LiamCrazedCreatorPower>(),
    };

    public LiamCrazedCreator() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int amount = (int)DynamicVars["MagicNumber"].BaseValue;
        await PowerCmd.Apply<LiamCrazedCreatorPower>(Owner.Creature, amount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(2m);
    }
}
