using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;

/// <summary>
/// T2 Ominous Artifact α — Deal 14 (+1 per card in exhaust pile) damage to target enemy.
/// Deal 14 damage to ALL other enemies.
/// </summary>
public sealed class OminousArtifactAlpha : ArtifactCard
{
    public override ArtifactTier Tier => ArtifactTier.T2_Steel;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CalculationBaseVar(14m),
        new ExtraDamageVar(2m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel card, Creature? _) =>
                card.Owner != null ? PileType.Exhaust.GetPile(card.Owner).Cards.Count : 0),
        new IntVar("MagicNumber", 14m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
    };

    public OminousArtifactAlpha() : base(2, ArtifactType.Artifact, TargetType.AnyEnemy) { }

    protected override void OnUpgrade()
    {
        DynamicVars["CalculationBase"].UpgradeValueBy(4m);
        DynamicVars["ExtraDamage"].UpgradeValueBy(1m);
        DynamicVars["MagicNumber"].UpgradeValueBy(4m);
    }

    protected override async Task OnRawPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
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
    }

    public override async Task ActivateEffect(PlayerChoiceContext choiceContext)
    {
        var enemies = CombatState.HittableEnemies;
        if (enemies.Count == 0) return;

        // When activated via Ralmia etc., no target selected — fall back to highest HP
        Creature target = enemies.MaxBy(e => e.CurrentHp)!;

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
}
