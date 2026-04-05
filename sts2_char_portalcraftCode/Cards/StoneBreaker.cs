using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Stone Breaker — 1 cost Uncommon Skill.
/// Do this 12 times: Deal 1 damage to a random enemy.
/// Upgrade: 16 times instead.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class StoneBreaker : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 12m),
    };

    public StoneBreaker() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hits = (int)DynamicVars["MagicNumber"].BaseValue;
        var enemies = CombatState.HittableEnemies.ToList();
        if (enemies.Count == 0) return;

        for (int i = 0; i < hits; i++)
        {
            var target = Owner.RunState.Rng.Shuffle.NextItem(enemies);
            await CreatureCmd.Damage(choiceContext, target, 1m, ValueProp.Unpowered, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(4m);
    }
}
