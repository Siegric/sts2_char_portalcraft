using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;
public sealed class AncientCannonPower : PortalcraftPower, IFuseListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnArtifactFused(
        PlayerChoiceContext choiceContext,
        ArtifactCard playedCard,
        IReadOnlyList<CardModel> discardedCards,
        CardModel resultCard,
        ArtifactTier resultTier)
    {
        if (playedCard.Owner.Creature != Owner) return;

        var target = CombatState.HittableEnemies.FirstOrDefault();
        if (target != null)
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, target, Amount, ValueProp.Unpowered, Owner, null);
        }
    }
}
