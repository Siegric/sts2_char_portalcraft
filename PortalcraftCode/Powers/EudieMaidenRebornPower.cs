using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class EudieMaidenRebornPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player) return;

        var handCount = PileType.Hand.GetPile(Owner.Player).Cards.Count;

        switch (handCount)
        {
            case <= 5:
                Flash();
                await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner, Amount, Owner, null);
                break;
            case >= 6:
                Flash();
                await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner, Amount, Owner, null);
                break;
        }
    }
}
