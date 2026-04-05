using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Audio;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Patches;

/// <summary>
/// Plays custom audio when a card is played from hand.
/// Skips Talisman types (they play audio on add-to-hand instead).
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class CardPlayAudioPatch
{
    public static void Prefix(CardModel __instance)
    {
        var typeName = __instance.GetType().Name;
        if (CardPlayAudioManager.IsAddToHandType(typeName)) return;
        CardPlayAudioManager.PlayForCard(typeName);
    }
}

/// <summary>
/// Plays custom audio for Talisman tokens when they are added to the player's hand.
/// </summary>
[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.AddGeneratedCardToCombat))]
public static class TalismanAddToHandAudioPatch
{
    public static void Prefix(CardModel card, PileType newPileType)
    {
        if (newPileType != PileType.Hand) return;
        var typeName = card.GetType().Name;
        if (!CardPlayAudioManager.IsAddToHandType(typeName)) return;
        CardPlayAudioManager.PlayForCard(typeName);
    }
}
