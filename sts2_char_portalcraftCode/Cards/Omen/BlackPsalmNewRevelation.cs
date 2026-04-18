using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Omen;

public sealed class BlackPsalmNewRevelation : sts2_char_portalcraftCard, ICountdownCard, ILastWordsCard
{
    private const int BaseDamageAmount = 2;
    private const int UpgradeDamageAmount = 1;

    public int DamageValue => BaseDamageAmount + (CurrentUpgradeLevel > 0 ? UpgradeDamageAmount : 0);

    protected override HashSet<CardTag> CanonicalTags => new() { OmenTag.Amulet };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Retain,
        CardKeyword.Unplayable,
        CountdownKeyword.Countdown,
        LastWordsKeyword.LastWords,
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar(CountdownHelper.CountdownVarName, 1m),
    };

    public BlackPsalmNewRevelation() : base(0, AmuletType.Amulet, CardRarity.Token, TargetType.Self, showInCardLibrary: true) { }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("PsalmDamage", DamageValue.ToString());
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    protected override void OnUpgrade() { }

    public async Task OnLastWords(PlayerChoiceContext choiceContext)
    {
        int damage = DamageValue + AmuletHelper.GetBeelzebubBonus(Owner.Creature);
        foreach (var enemy in CombatState.HittableEnemies.ToList())
        {
            await CreatureCmd.Damage(choiceContext, enemy, damage, ValueProp.Unpowered, Owner.Creature, null);
        }

        var white = CombatState.CreateCard<WhitePsalmNewRevelation>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(white, PileType.Hand, addedByPlayer: true);
    }

    public static async Task<CardModel> CreateInHand(Player owner, CombatState combatState)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return null;

        var psalm = combatState.CreateCard<BlackPsalmNewRevelation>(owner);
        await CardPileCmd.AddGeneratedCardToCombat(psalm, PileType.Hand, addedByPlayer: true);
        return psalm;
    }
}
