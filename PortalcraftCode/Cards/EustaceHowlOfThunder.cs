using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
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
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public class EustaceHowlOfThunder : PortalcraftCard, IEvolvableCard, ISkyboundArtCard
{
    protected readonly EvoTier Tier;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(18m, ValueProp.Move),
        new IntVar(SkyboundArtHelper.SkyboundArtVarName, 0m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        SkyboundArtKeyword.SkyboundArt,
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(SkyboundArtKeyword.SkyboundArt),
        HoverTipFactory.FromKeyword(EvolutionKeyword.Evolution),
    };

    public EustaceHowlOfThunder() : this(EvoTier.Base) { }
    protected EustaceHowlOfThunder(EvoTier tier)
        : base(2, CardType.Attack, tier.OverrideRarity(CardRarity.Uncommon), TargetType.AnyEnemy,
               showInCardLibrary: tier == EvoTier.Base)
    {
        Tier = tier;
    }

    public virtual Type? EvolvedType      => Tier == EvoTier.Base ? typeof(EustaceHowlOfThunderEvolved)      : null;
    public virtual Type? SuperEvolvedType => Tier == EvoTier.Base ? typeof(EustaceHowlOfThunderSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => Tier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{Tier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }
    
    public async Task OnSkyboundArt(CardModel card, PlayerChoiceContext choiceContext)
    {
        bool Filter(CardModel c) => c != this
                                 && c is IEvolvableCard
                                 && EvoRuntime.GetTier(c) == null;

        var candidates = PileType.Hand.GetPile(Owner).Cards.Where(Filter).ToList();
        if (candidates.Count > 0)
        {
            var prefs = new CardSelectorPrefs(
                new LocString("card_selection", "EUSTACE_SKYBOUND_PROMPT"),
                minCount: 1,
                maxCount: 1);

            var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
            if (selected.Count > 0)
            {
                await EvoCmd.ForceEvolve(selected[0], choiceContext, playVfx: true);
            }
        }

        if (EvoRuntime.GetTier(this) == null)
        {
            await EvoCmd.PlayEvolveVfx(this);
            EvoRuntime.MarkEvolved(this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
