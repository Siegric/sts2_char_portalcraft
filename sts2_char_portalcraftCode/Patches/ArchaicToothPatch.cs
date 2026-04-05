using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Patches;

/// <summary>
/// Patches ArchaicTooth to register Artifact Recharge → Myriad Designs
/// as a valid Transcendence transformation.
/// </summary>
[HarmonyPatch(typeof(ArchaicTooth))]
public static class ArchaicToothPatch
{
    /// <summary>
    /// Patches get_TranscendenceCards to include Biofabrication in the list
    /// so the card library / exclusion logic recognizes it as a transcendence card.
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ArchaicTooth.TranscendenceCards), MethodType.Getter)]
    public static void TranscendenceCards_Postfix(ref List<CardModel> __result)
    {
        __result.Add(ModelDb.Card<Biofabrication>());
    }

    /// <summary>
    /// Patches SetupForPlayer to handle our custom character.
    /// If the player has an ArtifactRecharge in their deck, the relic will
    /// set up to transform it into Biofabrication.
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ArchaicTooth.SetupForPlayer))]
    public static void SetupForPlayer_Postfix(ArchaicTooth __instance, ref bool __result,
        MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        // If the base method already found a valid starter card, don't override
        if (__result) return;

        // Look for ArtifactRecharge in the player's deck
        foreach (var card in player.Deck.Cards)
        {
            if (card.Id == ModelDb.Card<ArtifactRecharge>().Id)
            {
                // Use reflection to set the private properties via Harmony's Traverse
                var traverse = Traverse.Create(__instance);

                var ancientCard = player.RunState.CreateCard(ModelDb.Card<Biofabrication>(), player);
                if (card.IsUpgraded)
                {
                    MegaCrit.Sts2.Core.Commands.CardCmd.Upgrade(ancientCard);
                }

                traverse.Property("StarterCard").SetValue(card.ToSerializable());
                traverse.Property("AncientCard").SetValue(ancientCard.ToSerializable());

                __result = true;
                return;
            }
        }
    }

}

/// <summary>
/// Separate patch class for the private GetTranscendenceTransformedCard method.
/// This ensures that when ArchaicTooth looks up the transformation for ArtifactRecharge,
/// it returns Biofabrication instead of falling through to the default Doubt card.
/// </summary>
[HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceTransformedCard")]
public static class ArchaicToothTransformPatch
{
    public static bool Prefix(ArchaicTooth __instance, CardModel starterCard, ref CardModel __result)
    {
        if (starterCard.Id != ModelDb.Card<ArtifactRecharge>().Id)
            return true; // Let the original method handle it

        // Create the Biofabrication card for this player
        var owner = Traverse.Create(__instance).Property("Owner").GetValue<MegaCrit.Sts2.Core.Entities.Players.Player>();
        var ancientCard = owner.RunState.CreateCard(ModelDb.Card<Biofabrication>(), owner);

        if (starterCard.IsUpgraded)
        {
            MegaCrit.Sts2.Core.Commands.CardCmd.Upgrade(ancientCard);
        }

        // Copy enchantment if present
        if (starterCard.Enchantment != null)
        {
            var enchantment = (MegaCrit.Sts2.Core.Models.EnchantmentModel)starterCard.Enchantment.MutableClone();
            MegaCrit.Sts2.Core.Commands.CardCmd.Enchant(enchantment, ancientCard, enchantment.Amount);
        }

        __result = ancientCard;
        return false; // Skip the original method
    }
}

/// <summary>
/// Patches GetTranscendenceStarterCard to also recognize ArtifactRecharge as a valid starter.
/// </summary>
[HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceStarterCard")]
public static class ArchaicToothStarterPatch
{
    public static void Postfix(ref CardModel? __result,
        MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        // If the base method already found a valid starter, don't override
        if (__result != null) return;

        // Check if the player has an ArtifactRecharge
        var artifactRechargeId = ModelDb.Card<ArtifactRecharge>().Id;
        foreach (var card in player.Deck.Cards)
        {
            if (card.Id == artifactRechargeId)
            {
                __result = card;
                return;
            }
        }
    }
}
