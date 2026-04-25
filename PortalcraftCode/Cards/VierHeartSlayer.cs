using System;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public class VierHeartSlayer : PortalcraftCard, IEvolvableCard
{
    [SavedProperty]
    public EvoTier sts2_char_portalcraft_CurrentTier { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<DollSlayer>(),
        HoverTipFactory.FromKeyword(BaneKeyword.Bane),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
        BaneKeyword.Bane,
    };

    public VierHeartSlayer() : this(EvoTier.Base) { }

    protected VierHeartSlayer(EvoTier tier)
        : base(1, CardType.Skill, tier.OverrideRarity(CardRarity.Common), TargetType.AnyEnemy,
               showInCardLibrary: tier == EvoTier.Base)
    {
        sts2_char_portalcraft_CurrentTier = tier;
    }

    public virtual Type? EvolvedType      => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(VierHeartSlayerEvolved)      : null;
    public virtual Type? SuperEvolvedType => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(VierHeartSlayerSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => sts2_char_portalcraft_CurrentTier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        
        bool Filter(CardModel c) => c != this && PuppetHelper.IsPuppet(c);

        var handPuppets = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (handPuppets.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "VIER_HEART_SLAYER_PROMPT"),
            minCount: 0,
            maxCount: 1
        );

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;

        foreach (var puppet in selected)
        {
            decimal puppetDamage = puppet.DynamicVars.Damage?.BaseValue ?? 0m;

            await CardCmd.Exhaust(choiceContext, puppet);
            var dollSlayer = CombatState.CreateCard<DollSlayer>(Owner);
            dollSlayer.DynamicVars.Damage.BaseValue = puppetDamage;
            await CardPileCmd.AddGeneratedCardToCombat(dollSlayer, PileType.Hand, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
