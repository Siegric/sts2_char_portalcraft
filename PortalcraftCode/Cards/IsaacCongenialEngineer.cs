using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public class IsaacCongenialEngineer : PortalcraftCard, IEvolvableCard
{
    protected readonly EvoTier Tier;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(4m, ValueProp.Move),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<StrikerArtifact>(),
    };

    public IsaacCongenialEngineer() : this(EvoTier.Base) { }

    protected IsaacCongenialEngineer(EvoTier tier)
        : base(1, CardType.Skill, tier.OverrideRarity(CardRarity.Uncommon), TargetType.Self,
               showInCardLibrary: tier == EvoTier.Base)
    {
        Tier = tier;
    }

    public virtual Type? EvolvedType      => Tier == EvoTier.Base ? typeof(IsaacCongenialEngineerEvolved)      : null;
    public virtual Type? SuperEvolvedType => Tier == EvoTier.Base ? typeof(IsaacCongenialEngineerSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => Tier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        var striker = CombatState.CreateCard<StrikerArtifact>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(striker, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
