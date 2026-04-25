using System;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public class SubstandardPuppet : PortalcraftCard, IEvolvableCard
{
    [SavedProperty]
    public EvoTier sts2_char_portalcraft_CurrentTier { get; set; }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(SummonKeyword.Summon)
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(14m, ValueProp.Move),
    };
    public SubstandardPuppet() : this(EvoTier.Base) { }
    protected SubstandardPuppet(EvoTier tier)
        : base(2, CardType.Attack, tier.OverrideRarity(CardRarity.Uncommon), TargetType.AnyEnemy,
               showInCardLibrary: tier == EvoTier.Base)
    {
        sts2_char_portalcraft_CurrentTier = tier;
    }

    public virtual Type? EvolvedType      => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(SubstandardPuppetEvolved)      : null;
    public virtual Type? SuperEvolvedType => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(SubstandardPuppetSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => sts2_char_portalcraft_CurrentTier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        if (EvoRuntime.GetTier(this) == null)
        {
            await EvoCmd.PlayEvolveVfx(this);
            await EvoRuntime.MarkEvolved(this, choiceContext);
        }
        if (!SummonHelper.IsSummoned(this))
        {
            var copy = CombatState.CreateCard<SubstandardPuppet>(Owner);
            await SummonHelper.Summon(copy, Owner);
            await EvoRuntime.MarkEvolved(copy, choiceContext);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
