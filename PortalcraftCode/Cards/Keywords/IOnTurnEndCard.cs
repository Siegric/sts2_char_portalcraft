using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

// Cards in hand (draw, hand, discard) receive this callback once per player
// turn end, fired from KeywordDispatcherPower.BeforeTurnEnd before any flush.
public interface IOnTurnEndCard
{
    Task OnTurnEnd(PlayerChoiceContext choiceContext);
}
