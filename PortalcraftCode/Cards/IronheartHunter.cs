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

public class IronheartHunter : PortalcraftCard, IEvolvableCard
{
    protected readonly EvoTier Tier;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(BaseDamage, ValueProp.Move),
    };
    
    protected virtual decimal BaseDamage => 6m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<GearOfAmbition>(),
        HoverTipFactory.FromKeyword(EvolutionKeyword.Evolution),
        HoverTipFactory.FromKeyword(EvolveKeyword.Evolve),
    };

    public IronheartHunter() : this(EvoTier.Base) { }

    protected IronheartHunter(EvoTier tier)
        : base(1, CardType.Attack, tier.OverrideRarity(CardRarity.Common), TargetType.AnyEnemy,
               showInCardLibrary: tier == EvoTier.Base)
    {
        Tier = tier;
    }

    public virtual Type? EvolvedType      => Tier == EvoTier.Base ? typeof(IronheartHunterEvolved)      : null;
    public virtual Type? SuperEvolvedType => Tier == EvoTier.Base ? typeof(IronheartHunterSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => Tier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        var token = CombatState.CreateCard<GearOfAmbition>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(token, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
