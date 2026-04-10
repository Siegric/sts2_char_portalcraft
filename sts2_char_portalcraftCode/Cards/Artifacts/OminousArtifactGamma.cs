using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;

/// <summary>
/// T2 Ominous Artifact γ — Draw 4 cards. Gain 4 Energy. Next turn, draw 2 additional cards.
/// </summary>
public sealed class OminousArtifactGamma : ArtifactCard
{
    public override ArtifactTier Tier => ArtifactTier.T2_Steel;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(4),
        new EnergyVar(4),
        new IntVar("MagicNumber", 2m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<DrawCardsNextTurnPower>(),
    };

    public OminousArtifactGamma() : base(2, ArtifactType.Artifact, TargetType.Self) { }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars.Energy.UpgradeValueBy(1);
        DynamicVars["MagicNumber"].UpgradeValueBy(1m);
    }

    protected override async Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            Owner.Creature, DynamicVars["MagicNumber"].BaseValue, Owner.Creature, this);
    }

    public override async Task ActivateEffect(PlayerChoiceContext choiceContext)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            Owner.Creature, DynamicVars["MagicNumber"].BaseValue, Owner.Creature, this);
    }
}
