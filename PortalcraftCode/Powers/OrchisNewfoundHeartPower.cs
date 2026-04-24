using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Audio;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

// Turn-scoped: on apply, gives Replay 1 + Bane to every puppet currently in
// hand. Also applies to puppets added to hand later this turn (draw or
// generated). Removes itself at end of the player's turn.
public sealed class OrchisNewfoundHeartPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var player = Owner.Player;
        if (player == null) return Task.CompletedTask;

        foreach (var card in PileType.Hand.GetPile(player).Cards.ToList())
        {
            ApplyIfPuppet(card);
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        ApplyIfPuppet(card);
        return Task.CompletedTask;
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        ApplyIfPuppet(card);
        return Task.CompletedTask;
    }

    public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player)
        {
            Owner.RemovePowerInternal(this);
        }
        return Task.CompletedTask;
    }

    private void ApplyIfPuppet(CardModel card)
    {
        if (card.Owner?.Creature != Owner) return;
        if (!PuppetHelper.IsPuppet(card)) return;

        Flash();
        CardPlayAudioManager.PlayForEffect("OrchisNewfoundHeart");
        card.BaseReplayCount += 1;
        card.AddKeyword(BaneKeyword.Bane);
    }
}
