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

    // Mass-refresh the displayed gauge var on every Skybound Art card in the
    // player's hand. Call after any change to the global Skybound Art counter
    // so the counter value shown on each card stays in sync.
    public static void RefreshAllInHand(Player owner)
    {
        foreach (var card in PileType.Hand.GetPile(owner).Cards)
        {
            if (card is ISkyboundArtCard) RefreshGauge(card);
        }
    }
}
