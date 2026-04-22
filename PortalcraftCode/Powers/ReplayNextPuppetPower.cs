using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class ReplayNextPuppetPower : PortalcraftPower
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
            Owner.RemovePowerInternal(this);
        }
        return Task.CompletedTask;
    }
}
