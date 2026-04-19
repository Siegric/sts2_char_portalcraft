using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

public interface ILastWordsEnchantment
{
    Task OnLastWords(PlayerChoiceContext choiceContext, CardModel card);
}
