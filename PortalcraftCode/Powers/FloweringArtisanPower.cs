using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class FloweringArtisanPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner) return;
        if (cardPlay.Card.Type != CardType.Skill) return;
        if (cardPlay.Card is FloweringArtisan) return;

        Flash();
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await CreatureCmd.Damage(context, enemy, Amount, ValueProp.Unpowered, Owner, null);
        }
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
