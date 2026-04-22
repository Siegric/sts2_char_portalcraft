#if FALSE
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;
public sealed class RalmiaSonicRacerPower : PortalcraftPower, IFuseListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _fuseCount;

    public async Task OnArtifactFused(
        PlayerChoiceContext choiceContext,
        ArtifactCard playedCard,
        IReadOnlyList<CardModel> discardedCards,
        CardModel resultCard,
        ArtifactTier resultTier)
    {
        if (playedCard.Owner?.Creature != Owner) return;

        _fuseCount++;
        if (_fuseCount >= 4)
        {
            _fuseCount = 0;
            Flash();
            await PlayerCmd.GainEnergy(Amount, playedCard.Owner);
        }
    }
}
#endif
