using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

/// <summary>
/// Ralmia, Sonic Racer power: For every 4 fusions, gain 1 energy.
/// </summary>
public sealed class RalmiaSonicRacerPower : sts2_char_portalcraftPower, IMergeListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _mergeCount;

    public async Task OnArtifactMerged(
        PlayerChoiceContext choiceContext,
        ArtifactCard playedCard,
        IReadOnlyList<CardModel> discardedCards,
        CardModel resultCard,
        ArtifactTier resultTier)
    {
        if (playedCard.Owner?.Creature != Owner) return;

        _mergeCount++;
        if (_mergeCount >= 4)
        {
            _mergeCount = 0;
            Flash();
            await PlayerCmd.GainEnergy(Amount, playedCard.Owner);
        }
    }
}
