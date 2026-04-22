using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class MechanizedBeastPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || !props.HasFlag(ValueProp.Move) || props.HasFlag(ValueProp.Unpowered)) return;

        Flash();
        await CreatureCmd.Damage(choiceContext, dealer, Amount, ValueProp.Unpowered | ValueProp.SkipHurtAnim, Owner, null);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            Owner.RemovePowerInternal(this);
        }
        return Task.CompletedTask;
    }
}
