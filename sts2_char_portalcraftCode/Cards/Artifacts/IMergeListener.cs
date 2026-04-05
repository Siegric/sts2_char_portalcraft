using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;

/// <summary>
/// Interface for powers that react to artifact merge events.
/// Implemented by powers like Artifact Cannon, Assembly Line, Eternal Fabricator, Mass Production.
/// </summary>
public interface IMergeListener
{
    Task OnArtifactMerged(
        PlayerChoiceContext choiceContext,
        ArtifactCard playedCard,
        IReadOnlyList<CardModel> discardedCards,
        CardModel resultCard,
        ArtifactTier resultTier);
}
