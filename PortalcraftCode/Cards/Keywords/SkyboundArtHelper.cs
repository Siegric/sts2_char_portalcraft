using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class SkyboundArtHelper
{
    public const string SkyboundArtVarName = "SkyboundArt";

    public static void RefreshGauge(CardModel card)
    {
        if (!card.DynamicVars.ContainsKey(SkyboundArtVarName)) return;
        card.DynamicVars[SkyboundArtVarName].BaseValue = SkyboundArtRuntime.CurrentGauge(card);
    }
    
    public static void RefreshAllInHand(Player owner)
    {
        foreach (var card in PileType.Hand.GetPile(owner).Cards)
        {
            if (card is ISkyboundArtCard) RefreshGauge(card);
        }
    }
}
