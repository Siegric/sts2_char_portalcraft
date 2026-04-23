using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Audio;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class OrchisNewfoundHeartPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

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
