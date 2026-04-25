using System;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public class RalmiaSonicBoom : PortalcraftCard, IEvolvableCard
{
    [SavedProperty]
    public EvoTier sts2_char_portalcraft_CurrentTier { get; set; }

    protected override bool HasEnergyCostX => true;

    public RalmiaSonicBoom() : this(EvoTier.Base) { }
    protected RalmiaSonicBoom(EvoTier tier)
        : base(-1, CardType.Skill, tier.OverrideRarity(CardRarity.Rare), TargetType.Self,
               showInCardLibrary: tier == EvoTier.Base)
    {
        sts2_char_portalcraft_CurrentTier = tier;
    }

    public virtual Type? EvolvedType      => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(RalmiaSonicBoomEvolved)      : null;
    public virtual Type? SuperEvolvedType => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(RalmiaSonicBoomSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => sts2_char_portalcraft_CurrentTier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selected = await SelectArtifacts(choiceContext);
        foreach (var card in selected)
        {
            if (card is ArtifactCard artifact)
            {
                await artifact.ActivateEffect(choiceContext);
            }
        }
    }
    
    protected async Task<List<CardModel>> SelectArtifacts(PlayerChoiceContext choiceContext)
    {
        int xValue = ResolveEnergyXValue();
        if (IsUpgraded)
            xValue++;

        if (xValue <= 0) return new List<CardModel>();

        bool Filter(CardModel c) =>
            c != this &&
            c is ArtifactCard &&
            c.EnergyCost.Canonical <= 2;

        var handArtifacts = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (handArtifacts.Count == 0) return new List<CardModel>();

        int maxSelect = Math.Min(xValue, handArtifacts.Count);
        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "RALMIA_PROMPT"),
            minCount: 0,
            maxCount: maxSelect
        );

        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this);
        return selected.ToList();
    }

    public virtual Task OnEvolve(CardModel card, PlayerChoiceContext choiceContext) => Task.CompletedTask;
    public virtual Task OnSuperEvolve(CardModel card, PlayerChoiceContext choiceContext) => Task.CompletedTask;

    protected override void OnUpgrade()
    {
    }
}
