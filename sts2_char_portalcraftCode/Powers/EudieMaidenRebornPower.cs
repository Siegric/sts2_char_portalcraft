using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

/// <summary>
/// Eudie, Maiden Reborn power:
/// At end of turn, if hand &lt; 5, draw +1 card next turn.
/// At end of turn, if hand &gt; 6, gain +1 Energy next turn.
/// </summary>
public sealed class EudieMaidenRebornPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player) return;

        var handCount = PileType.Hand.GetPile(Owner.Player).Cards.Count;

        if (handCount < 5)
        {
            Flash();
            await PowerCmd.Apply<DrawCardsNextTurnPower>(Owner, Amount, Owner, null);
        }

        if (handCount > 6)
        {
            Flash();
            await PowerCmd.Apply<EnergyNextTurnPower>(Owner, Amount, Owner, null);
        }
    }
}
