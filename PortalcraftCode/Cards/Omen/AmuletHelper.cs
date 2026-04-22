using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Omen;

public static class AmuletHelper
{
    public static int GetBeelzebubBonus(Creature owner)
    {
        var beelzebub = owner.GetPower<BeelzebubSupremeKingPower>();
        return beelzebub?.Amount ?? 0;
    }

    public static bool IsAmulet(CardModel card)
    {
        return card.Tags.Contains(OmenTag.Amulet);
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
}
