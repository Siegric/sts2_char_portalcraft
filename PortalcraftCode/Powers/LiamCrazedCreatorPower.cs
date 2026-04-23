using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class LiamCrazedCreatorPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner?.Creature != Owner) return;
        if (!PuppetHelper.IsPuppet(card)) return;

        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, cardPlay);

        if (card.CombatState == null) return;
        foreach (Creature enemy in card.CombatState.HittableEnemies.ToList())
        {
            await DamageCmd.Attack(Amount)
                .FromCard(card)
                .Targeting(enemy)
                .Execute(context);
        }
    }
}
