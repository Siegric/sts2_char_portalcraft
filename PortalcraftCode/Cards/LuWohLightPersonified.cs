using System;
using MegaCrit.Sts2.Core.Saves.Runs;
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
    [SavedProperty]
    public EvoTier sts2_char_portalcraft_CurrentTier { get; set; }

    private int HitCount => IsUpgraded ? 8 : 6;

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
        sts2_char_portalcraft_CurrentTier = tier;
    }

    public virtual Type? EvolvedType      => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(LuWohLightPersonifiedEvolved)      : null;
    public virtual Type? SuperEvolvedType => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(LuWohLightPersonifiedSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => sts2_char_portalcraft_CurrentTier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.HasPower<SkyboundArtAutoPlayingPower>())
        {
            await OnSkyboundArt(this, choiceContext);
            return;
        }

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
        await PowerCmd.Apply<LuWohIntentDebuffPower>(choiceContext, Owner.Creature, 3m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // Upgrade adds 2 more hits (6 → 8).
    }
}
