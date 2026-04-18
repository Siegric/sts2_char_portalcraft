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
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class RetrafiaDivineMother : sts2_char_portalcraftCard, ICrystallizeCard
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

    public RetrafiaDivineMother() : base(5, CardType.Skill, CardRarity.Ancient, TargetType.AllEnemies) { }

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
        await PowerCmd.Apply<RetrafiaAmuletPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade() { }
}
