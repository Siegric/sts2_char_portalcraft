using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

public sealed class SlausRevolvingWheelPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // Track which effect was last used to avoid repeating it
    private int _lastEffect = -1;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;

        Flash();

        // Build list of available effects (all except the last one used)
        var available = new List<int> { 0, 1, 2 };
        if (_lastEffect >= 0)
        {
            available.Remove(_lastEffect);
        }

        int index = player.RunState.Rng.Shuffle.NextInt(available.Count);
        int effect = available[index];
        _lastEffect = effect;

        switch (effect)
        {
            case 0:
                // Reduce all hand card costs by (1 * stacks) this turn
                var handCards = PileType.Hand.GetPile(player).Cards.ToList();
                foreach (var card in handCards)
                {
                    card.EnergyCost.AddThisTurn(-Amount);
                }
                break;

            case 1:
                // Gain (2 * stacks) Strength and (2 * stacks) Dexterity — this turn only.
                // Our concrete TemporaryStrength/Dexterity subclasses handle self-removal at turn end.
                var amount = 2m * Amount;
                await PowerCmd.Apply<SlausRevolvingWheelStrengthPower>(Owner, amount, Owner, null);
                await PowerCmd.Apply<SlausRevolvingWheelDexterityPower>(Owner, amount, Owner, null);
                break;

            case 2:
                // Heal (3 * stacks) HP
                await CreatureCmd.Heal(Owner, 3m * Amount);
                break;
        }
    }
}
