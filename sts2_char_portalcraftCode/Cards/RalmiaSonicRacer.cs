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
/// Ralmia, Sonic Racer — 2 cost Ancient Power.
/// Grant 1 energy for every 4 fusions.
/// Upgrade: Innate.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class RalmiaSonicRacer : sts2_char_portalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<RalmiaSonicRacerPower>(),
    };

    public RalmiaSonicRacer() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RalmiaSonicRacerPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
