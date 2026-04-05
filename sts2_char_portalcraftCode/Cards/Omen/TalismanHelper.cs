using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Omen;

public static class TalismanHelper
{
    public static bool IsTalisman(CardModel card)
    {
        return card.Tags.Contains(OmenTag.Talisman);
    }

    public static bool IsWastelandToken(CardModel card)
    {
        return card.Tags.Contains(OmenTag.WastelandToken);
    }

    public static bool IsWhitePsalm(CardModel card)
    {
        return card is WhitePsalmNewRevelation;
    }

    public static bool IsBlackPsalm(CardModel card)
    {
        return card is BlackPsalmNewRevelation;
    }

    public static async Task EnsureTalismanPower(Player owner, CardModel source)
    {
        if (!owner.Creature.HasPower<TalismanPower>())
        {
            await PowerCmd.Apply<TalismanPower>(owner.Creature, 1, owner.Creature, source);
        }
    }
}
