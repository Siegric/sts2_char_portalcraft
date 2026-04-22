using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class AugmentationBestowalPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner) return;
        if (cardPlay.Card is not ArtifactCard) return;

        Flash();
        await PlayerCmd.GainEnergy(1, cardPlay.Card.Owner);
        await CardPileCmd.Draw(choiceContext, 1, cardPlay.Card.Owner);
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
