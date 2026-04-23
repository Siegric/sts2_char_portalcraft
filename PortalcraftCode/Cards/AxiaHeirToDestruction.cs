using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public class AxiaHeirToDestruction : PortalcraftCard, IEvolvableCard
{
    protected readonly EvoTier Tier;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(9m, ValueProp.Move),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CannotBeExhaustedKeyword.CannotBeExhausted,
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(CannotBeExhaustedKeyword.CannotBeExhausted),
        HoverTipFactory.FromKeyword(SuperEvolveKeyword.SuperEvolve),
    };

    public AxiaHeirToDestruction() : this(EvoTier.Base) { }

    protected AxiaHeirToDestruction(EvoTier tier)
        : base(2, CardType.Skill, tier.OverrideRarity(CardRarity.Uncommon), TargetType.Self,
               showInCardLibrary: tier == EvoTier.Base)
    {
        Tier = tier;
    }

    public virtual Type? EvolvedType      => Tier == EvoTier.Base ? typeof(AxiaHeirToDestructionEvolved)      : null;
    public virtual Type? SuperEvolvedType => Tier == EvoTier.Base ? typeof(AxiaHeirToDestructionSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => Tier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    public virtual Task OnEvolve(CardModel card, PlayerChoiceContext choiceContext) => Task.CompletedTask;
    
    public virtual async Task OnSuperEvolve(CardModel card, PlayerChoiceContext choiceContext)
    {
        var otherCards = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        int x = otherCards.Count;

        var target = CombatState.HittableEnemies
            .OrderByDescending(e => e.CurrentHp)
            .FirstOrDefault();

        if (x > 0 && target != null)
        {
            await DamageCmd.Attack(5m * x)
                .FromCard(this)
                .Targeting(target)
                .Execute(choiceContext);
        }

        foreach (var c in otherCards)
        {
            await CardCmd.Exhaust(choiceContext, c);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
