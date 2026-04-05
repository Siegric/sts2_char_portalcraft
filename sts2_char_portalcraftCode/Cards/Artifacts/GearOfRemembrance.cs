using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;

/// <summary>T0 Gear — No effect. Merge with another Gear of Remembrance to create Fortifier Artifact.</summary>
public sealed class GearOfRemembrance : ArtifactCard
{
    public override ArtifactTier Tier => ArtifactTier.T0_Artifact;

    public GearOfRemembrance() : base(ArtifactType.Artifact, TargetType.Self) { }

    protected override Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}
