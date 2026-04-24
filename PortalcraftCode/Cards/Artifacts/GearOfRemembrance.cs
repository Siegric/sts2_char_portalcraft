using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Omen;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
public sealed class GearOfRemembrance : ArtifactCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };
    public override ArtifactTier Tier => ArtifactTier.T0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<FortifierArtifact>(),
        HoverTipFactory.FromKeyword(FuseKeyword.Fuse),
    };

    public GearOfRemembrance() : base(AmuletType.Amulet, TargetType.Self) { }

    protected override Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}
