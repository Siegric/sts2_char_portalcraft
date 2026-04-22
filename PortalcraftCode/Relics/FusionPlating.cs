using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

namespace sts2_char_portalcraft.PortalcraftCode.Relics;

public sealed class FusionPlating : PortalcraftRelic, IFuseListener
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public async Task OnArtifactFused(
        PlayerChoiceContext choiceContext,
        ArtifactCard playedCard,
        IReadOnlyList<CardModel> discardedCards,
        CardModel resultCard,
        ArtifactTier resultTier)
    {
        if (playedCard.Owner != Owner) return;

        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, 2m, ValueProp.Unpowered, null);
    }
}
