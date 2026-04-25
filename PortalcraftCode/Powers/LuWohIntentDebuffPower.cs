using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class LuWohIntentDebuffPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private const int StrengthReduction = 4;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState)
    {
        if (side != CombatSide.Enemy) return;

        bool flashed = false;
        foreach (Creature enemy in combatState.HittableEnemies)
        {
            if (enemy.Monster?.IntendsToAttack ?? false)
            {
                if (!flashed)
                {
                    Flash();
                    flashed = true;
                }

                await PowerCmd.Apply<LuWohStrengthPower>(choiceContext, enemy, StrengthReduction, Owner, null);
            }
        }

        await PowerCmd.Decrement(this);
    }
}
