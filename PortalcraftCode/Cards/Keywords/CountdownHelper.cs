using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class CountdownHelper
{
    public const string CountdownVarName = "Countdown";

    public static bool TryGetCountdown(CardModel card, out int countdown)
    {
        countdown = 0;
        if (card is not ICountdownCard) return false;
        if (!card.DynamicVars.ContainsKey(CountdownVarName)) return false;
        countdown = card.DynamicVars[CountdownVarName].IntValue;
        return true;
    }

    public static void SetCountdown(CardModel card, int value)
    {
        if (!card.DynamicVars.ContainsKey(CountdownVarName)) return;
        card.DynamicVars[CountdownVarName].BaseValue = value;
    }

    // Decrements a card's Countdown by 1; if it hits 0, exhausts the card (which fires
    // any Last Words via the dispatcher). Safe to call from any combat-time context.
    public static async Task Tick(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (!TryGetCountdown(card, out int current)) return;

        int next = current - 1;
        SetCountdown(card, next);

        if (next <= 0)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }
}
