using System;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public class PuppetCat : PortalcraftCard, IEvolvableCard
{
    [SavedProperty]
    public EvoTier sts2_char_portalcraft_CurrentTier { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move),
        new IntVar("PuppetBonus", 3m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Puppet>(),
    };

    public PuppetCat() : this(EvoTier.Base) { }

    protected PuppetCat(EvoTier tier)
        : base(1, CardType.Attack, tier.OverrideRarity(CardRarity.Basic), TargetType.AnyEnemy,
               showInCardLibrary: tier == EvoTier.Base)
    {
        sts2_char_portalcraft_CurrentTier = tier;
    }

    public virtual Type? EvolvedType      => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(PuppetCatEvolved)      : null;
    public virtual Type? SuperEvolvedType => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(PuppetCatSuperEvolved) : null;

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
        
        bool hasSuperEvolvedAlly = PileType.Hand.GetPile(Owner).Cards.Any(EvoRuntime.IsSuperEvolved);
        if (!hasSuperEvolvedAlly) return;

        var puppet = CombatState.CreateCard<Puppet>(Owner);
        puppet.DynamicVars.Damage.BaseValue += DynamicVars["PuppetBonus"].BaseValue;
        await CardPileCmd.AddGeneratedCardToCombat(puppet, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
