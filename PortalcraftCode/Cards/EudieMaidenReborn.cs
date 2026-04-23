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
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Extensions;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public class EudieMaidenReborn : PortalcraftCard, IEvolvableCard
{
    protected readonly EvoTier Tier;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<EudieMaidenRebornPower>(),
        HoverTipFactory.FromKeyword(EvolutionKeyword.Evolution),
        HoverTipFactory.FromKeyword(EvolveKeyword.Evolve),
    };

    public EudieMaidenReborn() : this(EvoTier.Base) { }

    protected EudieMaidenReborn(EvoTier tier)
        : base(1, CardType.Power, tier.OverrideRarity(CardRarity.Rare), TargetType.Self,
               showInCardLibrary: tier == EvoTier.Base)
    {
        Tier = tier;
    }

    public virtual Type? EvolvedType      => Tier == EvoTier.Base ? typeof(EudieMaidenRebornEvolved)      : null;
    public virtual Type? SuperEvolvedType => Tier == EvoTier.Base ? typeof(EudieMaidenRebornSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => Tier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
