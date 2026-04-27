using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;

public sealed class MasterworkOmega : ArtifactCard
{
    public override ArtifactTier Tier => ArtifactTier.T3;
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(14m),
        new ExtraDamageVar(3m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel card, Creature? _) =>
                card.Owner != null ? PileType.Exhaust.GetPile(card.Owner).Cards.Count : 0),
        new IntVar("MagicNumber", 14m),
        new HealVar(4m),
        new BlockVar(14m, ValueProp.Move),
        new PowerVar<PlatingPower>(6m),
        new CardsVar(4),
        new EnergyVar(4),
        new IntVar("DrawNextTurn", 2m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<PlatingPower>(),
        HoverTipFactory.FromPower<DrawCardsNextTurnPower>(),
    };

    public MasterworkOmega() : base(3, ArtifactType.Artifact, TargetType.AnyEnemy) { }

    protected override void OnUpgrade()
    {
        // Alpha portion
        DynamicVars["CalculationBase"].UpgradeValueBy(4m);
        DynamicVars["ExtraDamage"].UpgradeValueBy(1m);
        DynamicVars["MagicNumber"].UpgradeValueBy(4m);
        // Beta portion
        DynamicVars.Heal.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["PlatingPower"].UpgradeValueBy(2m);
        // Gamma portion
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars.Energy.UpgradeValueBy(1);
        DynamicVars["DrawNextTurn"].UpgradeValueBy(1m);
    }

    protected override async Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // --- Decimator portion ---
        var target = cardPlay.Target!;

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(target)
            .Execute(choiceContext);

        var others = CombatState.HittableEnemies.Where(e => e != target).ToList();
        foreach (var enemy in others)
        {
            await DamageCmd.Attack(DynamicVars["MagicNumber"].BaseValue)
                .FromCard(this)
                .Targeting(enemy)
                .Execute(choiceContext);
        }

        // --- Guardian portion ---
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<PlatingPower>(
            Owner.Creature, DynamicVars["PlatingPower"].BaseValue, Owner.Creature, this);

        // --- Processor portion ---
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            Owner.Creature, DynamicVars["DrawNextTurn"].BaseValue, Owner.Creature, this);
    }

    public override async Task ActivateEffect(PlayerChoiceContext choiceContext)
    {
        var enemies = CombatState.HittableEnemies;
        if (enemies.Count > 0)
        {
            // When activated via Ralmia etc., no target selected — fall back to lowest HP
            Creature target = enemies.MinBy(e => e.CurrentHp)!;

            await DamageCmd.Attack(DynamicVars.CalculatedDamage)
                .FromCard(this)
                .Targeting(target)
                .Execute(choiceContext);

            var others = CombatState.HittableEnemies.Where(e => e != target).ToList();
            foreach (var enemy in others)
            {
                await DamageCmd.Attack(DynamicVars["MagicNumber"].BaseValue)
                    .FromCard(this)
                    .Targeting(enemy)
                    .Execute(choiceContext);
            }
        }

        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, null);
        await PowerCmd.Apply<PlatingPower>(
            Owner.Creature, DynamicVars["PlatingPower"].BaseValue, Owner.Creature, this);

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            Owner.Creature, DynamicVars["DrawNextTurn"].BaseValue, Owner.Creature, this);
    }
}
