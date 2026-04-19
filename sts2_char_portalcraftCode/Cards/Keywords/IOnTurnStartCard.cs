using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

// Cards implementing this get their OnTurnStart invoked by KeywordDispatcherPower
// at the start of each player turn, before any Countdown ticks for this turn.
public interface IOnTurnStartCard
{
    Task OnTurnStart(PlayerChoiceContext choiceContext);
}
