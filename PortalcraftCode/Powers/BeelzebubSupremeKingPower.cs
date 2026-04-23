using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class BeelzebubSupremeKingPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private static readonly AsyncLocal<bool> _inBonusDamage = new();

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult results,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (_inBonusDamage.Value) return;
        if (dealer != Owner) return;
        if (target.Side != CombatSide.Enemy) return;
        if (!target.IsHittable) return;

        _inBonusDamage.Value = true;
        try
        {
            Flash();
            var cmd = DamageCmd.Attack(Amount).Targeting(target);
            if (cardSource != null) cmd = cmd.FromCard(cardSource);
            await cmd.Execute(choiceContext);
        }
        finally
        {
            _inBonusDamage.Value = false;
        }
    }
}
