using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public interface IOnTurnStartCard
{
    Task OnTurnStart(PlayerChoiceContext choiceContext);
}
