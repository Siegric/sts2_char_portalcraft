using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Audio;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class CardPlayAudioPatch
{
    [HarmonyPriority(Priority.First)]
    public static void Prefix(CardModel __instance)
    {
        var typeName = __instance.GetType().Name;
        if (CardPlayAudioManager.IsAddToHandType(typeName)) return;

        bool crystallizing = __instance is ICrystallizeCard && CrystallizeRuntime.IsActive(__instance);
        CardPlayAudioManager.PlayForCard(crystallizing ? typeName + "Crystallize" : typeName);
    }
}
[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.AddGeneratedCardToCombat))]
public static class AmuletAddToHandAudioPatch
{
    public static void Prefix(CardModel card, PileType newPileType)
    {
        if (newPileType != PileType.Hand) return;
        var typeName = card.GetType().Name;
        if (!CardPlayAudioManager.IsAddToHandType(typeName)) return;
        CardPlayAudioManager.PlayForCard(typeName);
    }
}
