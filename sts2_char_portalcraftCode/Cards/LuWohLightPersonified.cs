using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class LuWohLightPersonified : sts2_char_portalcraftCard
{
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (CombatState == null) return false;
            return EnergyCost.GetResolved() != 2;
        }
    }

    private int BaseStrLoss => IsUpgraded ? 3 : 2;
    private int BoostedStrLoss => IsUpgraded ? 5 : 4;
    private int HitCount => IsUpgraded ? 7 : 6;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 4m),
        new IntVar("BoostedMagicNumber", 6m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<LuWohIntentDebuffPower>(),
    };

    public LuWohLightPersonified() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (int i = 0; i < HitCount; i++)
        {
            var enemies = CombatState.HittableEnemies;
            if (enemies.Count == 0) break;

            Creature target = Owner.RunState.Rng.Shuffle.NextItem(enemies);
            await DamageCmd.Attack(1m)
                .FromCard(this)
                .Targeting(target)
                .Execute(choiceContext);
        }
        
        bool boosted = EnergyCost.GetResolved() != 2;

        if (boosted)
        {
            await PowerCmd.Apply<LuWohIntentDebuffPower>(Owner.Creature, 3m, Owner.Creature, this);
        }
        else
        {
            foreach (Creature enemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<LuWohStrengthPower>(enemy, BaseStrLoss, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(1m);
        DynamicVars["BoostedMagicNumber"].UpgradeValueBy(1m);
    }
}
