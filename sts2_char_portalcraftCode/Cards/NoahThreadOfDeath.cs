using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Noah, Thread of Death — 0 cost Skill.
/// All Puppets in your hand gain +3 damage.
/// Upgrade: +5 damage instead.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class NoahThreadOfDeath : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 7m),
    };

    public NoahThreadOfDeath() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal damageBonus = DynamicVars["MagicNumber"].BaseValue;
        var puppetsInHand = PileType.Hand.GetPile(Owner).Cards
            .Where(c => PuppetHelper.IsPuppet(c) && c.DynamicVars.Damage != null)
            .ToList();

        foreach (var puppet in puppetsInHand)
        {
            puppet.DynamicVars.Damage.UpgradeValueBy(damageBonus);
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(3m);
    }
}
