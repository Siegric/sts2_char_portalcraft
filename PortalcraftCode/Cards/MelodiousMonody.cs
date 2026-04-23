using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

// Token summoned by Lishenna, Melody Manifest. Picks a hand card to exhaust,
// then deals a flat 12 damage to a random enemy. Not evolvable, no pool.
public sealed class MelodiousMonody : PortalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(12m, ValueProp.Move),
    };

    public MelodiousMonody()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self, showInCardLibrary: true) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool AnyCard(CardModel c) => c != this;
        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(AnyCard).ToList();
        if (handCards.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "MELODIOUS_MONODY_PROMPT"),
            minCount: 1,
            maxCount: 1);

        var toExhaust = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, AnyCard, this)).ToList();
        if (toExhaust.Count == 0) return;

        foreach (var card in toExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        var enemies = CombatState.HittableEnemies.ToList();
        if (enemies.Count == 0) return;

        var target = Owner.RunState.Rng.Shuffle.NextItem(enemies);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(target)
            .Execute(choiceContext);
    }
}
