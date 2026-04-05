using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

/// <summary>
/// Temporary power: the next Puppet card played gets replayed (via BaseReplayCount).
/// Removes itself after triggering.
/// </summary>
public sealed class ReplayNextPuppetPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner?.Creature == Owner && PuppetHelper.IsPuppet(card))
        {
            Flash();
            card.BaseReplayCount += Amount;
            // Remove this power after triggering
            Owner.RemovePowerInternal(this);
        }
        return Task.CompletedTask;
    }
}
