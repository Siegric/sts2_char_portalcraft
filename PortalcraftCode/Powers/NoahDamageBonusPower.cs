using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class NoahDamageBonusPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner) return 0m;
        if (!props.HasFlag(ValueProp.Move)) return 0m;
        if (cardSource == null || !PuppetHelper.IsPuppet(cardSource)) return 0m;
        return Amount;
    }
    public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player)
        {
            Owner.RemovePowerInternal(this);
        }
        return Task.CompletedTask;
    }
}
