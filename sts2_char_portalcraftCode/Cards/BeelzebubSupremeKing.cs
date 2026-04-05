using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Beelzebub, Supreme King — 2 cost Power, Rare.
/// All Talisman cards gain +2 to their effects (heal, block, and damage).
/// Upgrade: Cost -1.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class BeelzebubSupremeKing : sts2_char_portalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<BeelzebubSupremeKingPower>(),
    };

    public BeelzebubSupremeKing() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BeelzebubSupremeKingPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
