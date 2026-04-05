using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Supplicant of Destruction — 0 cost Attack, Common.
/// Select a card in your hand and exhaust it. Deal 6 damage to a random enemy.
/// Upgrade: +4 damage.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class SupplicantOfDestruction : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),
    };

    public SupplicantOfDestruction() : base(0, CardType.Attack, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Select a card to exhaust
        bool AnyCard(CardModel c) => c != this;
        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(AnyCard).ToList();
        if (handCards.Count == 0) return;

        var exhaustPrefs = new CardSelectorPrefs(
            new LocString("card_selection", "SUPPLICANT_PROMPT"),
            minCount: 1,
            maxCount: 1
        );

        var toExhaust = (await CardSelectCmd.FromHand(choiceContext, Owner, exhaustPrefs, AnyCard, this)).ToList();
        if (toExhaust.Count == 0) return;

        foreach (var card in toExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        // Deal damage to a random enemy
        var enemies = CombatState.HittableEnemies.ToList();
        if (enemies.Count > 0)
        {
            var target = Owner.RunState.Rng.Shuffle.NextItem(enemies);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(target)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
