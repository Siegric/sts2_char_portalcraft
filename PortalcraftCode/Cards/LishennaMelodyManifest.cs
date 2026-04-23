using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Omen;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public class LishennaMelodyManifest : PortalcraftCard, IEvolvableCard
{
    protected readonly EvoTier Tier;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CannotBeExhaustedKeyword.CannotBeExhausted,
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<MelodiousMonody>(),
        HoverTipFactory.FromCard<WhitePsalmNewRevelation>(),
        HoverTipFactory.FromKeyword(CannotBeExhaustedKeyword.CannotBeExhausted),
        HoverTipFactory.FromKeyword(EvolutionKeyword.Evolution),
        HoverTipFactory.FromKeyword(EvolveKeyword.Evolve),
    };

    public LishennaMelodyManifest() : this(EvoTier.Base) { }

    protected LishennaMelodyManifest(EvoTier tier)
        : base(2, CardType.Skill, tier.OverrideRarity(CardRarity.Rare), TargetType.Self,
               showInCardLibrary: tier == EvoTier.Base)
    {
        Tier = tier;
    }

    public virtual Type? EvolvedType      => Tier == EvoTier.Base ? typeof(LishennaMelodyManifestEvolved)      : null;
    public virtual Type? SuperEvolvedType => Tier == EvoTier.Base ? typeof(LishennaMelodyManifestSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => Tier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var monody = CombatState.CreateCard<MelodiousMonody>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(monody, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
