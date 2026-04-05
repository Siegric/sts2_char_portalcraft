using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Unfeeling Eld Axe — 3 cost Rare Skill.
/// Activates in hand: Whenever a card with base cost >= 2 is played, reduce this card's cost by 1 this turn.
/// On play: Upgrade a random unupgraded card in your hand. Deal 6 damage to a random enemy.
/// Upgrade: Cost 2.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class UnfeelingEldAxe : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 6m),
    };

    public UnfeelingEldAxe() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    // In-hand trigger: when this card enters combat, catch up with cards already played this turn
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || IsClone) return Task.CompletedTask;

        int count = CombatManager.Instance.History.CardPlaysFinished
            .Count(e => e.CardPlay.Card.EnergyCost.Canonical >= 2
                     && e.CardPlay.Card.Owner == Owner
                     && e.HappenedThisTurn(CombatState));
        if (count > 0)
        {
            EnergyCost.AddThisTurn(-count);
        }
        return Task.CompletedTask;
    }

    // In-hand trigger: reduce cost by 1 when a base cost >= 2 card is played
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner) return Task.CompletedTask;
        if (cardPlay.Card.EnergyCost.Canonical < 2) return Task.CompletedTask;

        EnergyCost.AddThisTurn(-1);
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Upgrade a random unupgraded card in hand
        var upgradable = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != this && c.IsUpgradable)
            .ToList();
        if (upgradable.Count > 0)
        {
            var target = Owner.RunState.Rng.Shuffle.NextItem(upgradable);
            CardCmd.Upgrade(target);
        }

        // Deal 6 damage to a random enemy
        var enemies = CombatState.HittableEnemies.ToList();
        if (enemies.Count > 0)
        {
            var enemy = Owner.RunState.Rng.Shuffle.NextItem(enemies);
            await CreatureCmd.Damage(choiceContext, enemy, DynamicVars["MagicNumber"].BaseValue,
                ValueProp.Unpowered, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
