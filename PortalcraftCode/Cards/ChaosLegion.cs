using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class ChaosLegion : PortalcraftCard, ISkyboundArtCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(18m, ValueProp.Move),
        new IntVar("SuperDamage", 36m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        SuperSkyboundArtKeyword.SuperSkyboundArt,
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(SuperSkyboundArtKeyword.SuperSkyboundArt),
    };

    public ChaosLegion() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.HasPower<SkyboundArtAutoPlayingPower>())
        {
            await OnSuperSkyboundArt(this, choiceContext);
            return;
        }

        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(enemy)
                .Execute(choiceContext);
        }
    }

    public async Task OnSuperSkyboundArt(CardModel card, PlayerChoiceContext choiceContext)
    {
        decimal amount = DynamicVars["SuperDamage"].BaseValue;
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await DamageCmd.Attack(amount)
                .FromCard(this)
                .Targeting(enemy)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["SuperDamage"].UpgradeValueBy(8m);
    }
}
