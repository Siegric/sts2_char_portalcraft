using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Extensions;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public class LuWohLightPersonified : PortalcraftCard, IEvolvableCard, ISkyboundArtCard
{
    protected readonly EvoTier Tier;

    private int HitCount => IsUpgraded ? 8 : 6;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar(SkyboundArtHelper.SkyboundArtVarName, 0m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
        SkyboundArtKeyword.SkyboundArt,
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<LuWohIntentDebuffPower>(),
        HoverTipFactory.FromKeyword(SkyboundArtKeyword.SkyboundArt),
    };

    public LuWohLightPersonified() : this(EvoTier.Base) { }
    protected LuWohLightPersonified(EvoTier tier)
        : base(2, CardType.Skill, tier.OverrideRarity(CardRarity.Rare), TargetType.Self,
               showInCardLibrary: tier == EvoTier.Base)
    {
        Tier = tier;
    }

    public virtual Type? EvolvedType      => Tier == EvoTier.Base ? typeof(LuWohLightPersonifiedEvolved)      : null;
    public virtual Type? SuperEvolvedType => Tier == EvoTier.Base ? typeof(LuWohLightPersonifiedSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => Tier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (int i = 0; i < HitCount; i++)
        {
            var minions = CombatState.HittableEnemies
                .Where(e => e.HasPower<MinionPower>())
                .ToList();
            if (minions.Count == 0) break;

            var target = Owner.RunState.Rng.Shuffle.NextItem(minions);
            await DamageCmd.Attack(1m)
                .FromCard(this)
                .Targeting(target)
                .Execute(choiceContext);
        }
    }
    
    public async Task OnSkyboundArt(CardModel card, PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<LuWohIntentDebuffPower>(Owner.Creature, 3m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // Upgrade adds 2 more hits (6 → 8).
    }
}
