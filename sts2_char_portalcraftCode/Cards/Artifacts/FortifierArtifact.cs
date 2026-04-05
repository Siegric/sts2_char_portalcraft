using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;

/// <summary>T1 Artifact — Gain 14 Block.</summary>
public sealed class FortifierArtifact : ArtifactCard
{
    public override ArtifactTier Tier => ArtifactTier.T1_Iron;
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(14m, ValueProp.Move),
    };

    public FortifierArtifact() : base(1, ArtifactType.Artifact, TargetType.Self) { }

    protected override async Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    public override async Task ActivateEffect(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, null);
    }
}
