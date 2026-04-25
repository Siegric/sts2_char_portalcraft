using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

public sealed class OminousArtifactBeta : ArtifactCard
{
    public override ArtifactTier Tier => ArtifactTier.T2;
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new HealVar(6m),
        new BlockVar(14m, ValueProp.Move),
        new PowerVar<PlatingPower>(6m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<PlatingPower>(),
    };

    public OminousArtifactBeta() : base(2, ArtifactType.Artifact, TargetType.Self) { }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["PlatingPower"].UpgradeValueBy(2m);
    }

    protected override async Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<PlatingPower>(
            choiceContext, Owner.Creature, DynamicVars["PlatingPower"].BaseValue, Owner.Creature, this);
    }

    public override async Task ActivateEffect(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, null);
        await PowerCmd.Apply<PlatingPower>(
            choiceContext, Owner.Creature, DynamicVars["PlatingPower"].BaseValue, Owner.Creature, this);
    }
}
