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
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public class NewAgeCartographer : PortalcraftCard, IEvolvableCard
{
    [SavedProperty]
    public EvoTier sts2_char_portalcraft_CurrentTier { get; set; }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<OminousArtifactBeta>(),
        HoverTipFactory.FromKeyword(SuperEvolveKeyword.SuperEvolve),
        HoverTipFactory.FromKeyword(SummonKeyword.Summon),
    };

    public NewAgeCartographer() : this(EvoTier.Base) { }

    protected NewAgeCartographer(EvoTier tier)
        : base(2, CardType.Skill, tier.OverrideRarity(CardRarity.Rare), TargetType.Self,
               showInCardLibrary: tier == EvoTier.Base)
    {
        sts2_char_portalcraft_CurrentTier = tier;
    }

    public virtual Type? EvolvedType      => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(NewAgeCartographerEvolved)      : null;
    public virtual Type? SuperEvolvedType => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(NewAgeCartographerSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => sts2_char_portalcraft_CurrentTier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var artifact = CombatState.CreateCard<OminousArtifactBeta>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(artifact, PileType.Hand, true);
    }

    public virtual Task OnEvolve(CardModel card, PlayerChoiceContext choiceContext) => Task.CompletedTask;
    
    public virtual async Task OnSuperEvolve(CardModel card, PlayerChoiceContext choiceContext)
    {
        bool Filter(CardModel c) => c != this
                                 && c is ArtifactCard
                                 && c.EnergyCost.Canonical <= 2;

        var artifacts = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (artifacts.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "NEW_AGE_CARTOGRAPHER_COPY_PROMPT"),
            minCount: 0,
            maxCount: 1);

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;

        await SummonHelper.SummonCopyOf(selected[0], Owner);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
