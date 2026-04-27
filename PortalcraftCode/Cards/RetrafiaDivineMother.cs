using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class RetrafiaDivineMother : PortalcraftCard, ICrystallizeCard
{
    private const int BaseDamageMultiplier = 3;
    private const int UpgradeDamageMultiplier = 4;

    private int DamageMultiplier => IsUpgraded ? UpgradeDamageMultiplier : BaseDamageMultiplier;

    public int CrystallizeCost => 1;
    public Type AmuletFormType => typeof(RetrafiaDivineMotherAmulet);

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
        CrystallizeKeyword.Crystallize,
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(CountdownKeyword.Countdown),
        HoverTipFactory.FromKeyword(LastWordsKeyword.LastWords),
    };

    public RetrafiaDivineMother() : base(3, CardType.Skill, CardRarity.Ancient, TargetType.AllEnemies) { }

    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (CombatState == null) return false;
            var pcs = Owner?.PlayerCombatState;
            if (pcs == null) return false;
            int fullCost = Math.Max(0, EnergyCost.GetWithModifiers(CostModifiers.All));
            return pcs.Energy < fullCost && pcs.Energy >= CrystallizeCost;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (await CrystallizeRuntime.TryExecute(this, choiceContext)) return;

        int x = Owner.Creature.GetPower<KeywordDispatcherPower>()?.ArtifactsExhaustedCount ?? 0;
        int damage = DamageMultiplier * x;
        if (damage <= 0) return;

        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
            await CreatureCmd.Damage(choiceContext, enemy, damage, ValueProp.Unpowered, Owner.Creature, this);
        }
    }

    public async Task OnAmuletSpawned(PlayerChoiceContext choiceContext, CardModel amulet)
    {
        await PowerCmd.Apply<RetrafiaAmuletPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade() { }
}
