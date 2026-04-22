using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

public sealed class GearOfRemembrance : ArtifactCard
{
    public override ArtifactTier Tier => ArtifactTier.T0;

    public GearOfRemembrance() : base(ArtifactType.Artifact, TargetType.Self) { }

    protected override Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}
