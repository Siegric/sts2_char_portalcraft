using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class PuppetCat : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("BonusDamage", 4m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Puppet>(),
    };

    public PuppetCat() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var puppet = CombatState.CreateCard<Puppet>(Owner);
        // Buff the puppet's damage by BonusDamage
        puppet.DynamicVars.Damage.BaseValue += DynamicVars["BonusDamage"].BaseValue;
        await CardPileCmd.AddGeneratedCardToCombat(puppet, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
