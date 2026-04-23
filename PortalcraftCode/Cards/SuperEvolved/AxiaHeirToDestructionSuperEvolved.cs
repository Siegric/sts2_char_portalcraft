using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;

// Super-Evolve: Deal 5X damage to the enemy with the highest HP, where X is
// the number of other cards in your hand. Then exhaust all other cards in
// your hand. Cards flagged CannotBeExhausted are skipped by the Harmony
// prefix in CannotBeExhaustedPatch.
public class AxiaHeirToDestructionSuperEvolved : AxiaHeirToDestructionEvolved
{
    public AxiaHeirToDestructionSuperEvolved() : base(EvoTier.SuperEvolved) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var otherCards = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        int x = otherCards.Count;

        var target = CombatState.HittableEnemies
            .OrderByDescending(e => e.CurrentHp)
            .FirstOrDefault();

        if (x > 0 && target != null)
        {
            await DamageCmd.Attack(5m * x)
                .FromCard(this)
                .Targeting(target)
                .Execute(choiceContext);
        }

        foreach (var c in otherCards)
        {
            await CardCmd.Exhaust(choiceContext, c);
        }
    }
}
