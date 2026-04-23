using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public class CamiscillaUnfeelingHeart : PortalcraftCard, IEvolvableCard
{
    protected readonly EvoTier Tier;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<ShoddyPlaything>(),
        HoverTipFactory.FromCard<SubstandardPuppet>(),
        HoverTipFactory.FromKeyword(EvolveKeyword.Evolve),
        HoverTipFactory.FromKeyword(SummonKeyword.Summon),
    };

    public CamiscillaUnfeelingHeart() : this(EvoTier.Base) { }
    protected CamiscillaUnfeelingHeart(EvoTier tier)
        : base(3, CardType.Skill, tier.OverrideRarity(CardRarity.Uncommon), TargetType.Self,
               showInCardLibrary: tier == EvoTier.Base)
    {
        Tier = tier;
    }

    public virtual Type? EvolvedType      => Tier == EvoTier.Base ? typeof(CamiscillaUnfeelingHeartEvolved)      : null;
    public virtual Type? SuperEvolvedType => Tier == EvoTier.Base ? typeof(CamiscillaUnfeelingHeartSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => Tier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await SummonHelper.Summon<ShoddyPlaything>(Owner, CombatState);
        await SummonHelper.Summon<SubstandardPuppet>(Owner, CombatState);
        
        var targets = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c is IEvolvableCard
                     && c.EnergyCost.Canonical >= 2
                     && EvoRuntime.GetTier(c) == null)
            .ToList();

        foreach (var c in targets)
        {
            await EvoCmd.ForceEvolve(c, choiceContext, playVfx: true);
        }
    }

    public virtual Task OnEvolve(CardModel card, PlayerChoiceContext choiceContext) => Task.CompletedTask;
    
    public virtual async Task OnSuperEvolve(CardModel card, PlayerChoiceContext choiceContext)
    {
        int x = PileType.Hand.GetPile(Owner).Cards
            .Count(c => c is IEvolvableCard && c.EnergyCost.Canonical >= 2);
        if (x <= 0) return;

        decimal damage = 5m * x;
        foreach (Creature enemy in CombatState.HittableEnemies.ToList())
        {
            await DamageCmd.Attack(damage)
                .FromCard(this)
                .Targeting(enemy)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
