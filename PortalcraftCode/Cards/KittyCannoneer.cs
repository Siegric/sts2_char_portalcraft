using System;
using MegaCrit.Sts2.Core.Saves.Runs;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public class KittyCannoneer : PortalcraftCard, IEvolvableCard
{
    [SavedProperty]
    public EvoTier sts2_char_portalcraft_CurrentTier { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, ValueProp.Move),
        new IntVar("MagicNumber", 1m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<GearOfAmbition>(),
    };

    public KittyCannoneer() : this(EvoTier.Base) { }

    protected KittyCannoneer(EvoTier tier)
        : base(1, CardType.Attack, tier.OverrideRarity(CardRarity.Uncommon), TargetType.AnyEnemy,
               showInCardLibrary: tier == EvoTier.Base)
    {
        sts2_char_portalcraft_CurrentTier = tier;
    }

    public virtual Type? EvolvedType      => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(KittyCannoneerEvolved)      : null;
    public virtual Type? SuperEvolvedType => sts2_char_portalcraft_CurrentTier == EvoTier.Base ? typeof(KittyCannoneerSuperEvolved) : null;

    public override bool CanBeGeneratedInCombat => sts2_char_portalcraft_CurrentTier == EvoTier.Base && base.CanBeGeneratedInCombat;

    public override string PortraitPath       => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string CustomPortraitPath => $"{sts2_char_portalcraft_CurrentTier.PortraitSubfolder()}{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        int debuffStacks = (int)DynamicVars["MagicNumber"].BaseValue;
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target!, debuffStacks, Owner.Creature, this);

        var gear = CombatState.CreateCard<GearOfAmbition>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(gear, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(1m);
    }
}
