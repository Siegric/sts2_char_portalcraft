using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Omen;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.PortalcraftCode.Patches;

[HarmonyPatch]
public static class CustomCardTypePatch
{
    private static CardType GetFrameType(CardModel card)
    {
        if (card.Type == PuppetType.Puppet) return CardType.Attack;
        if (card.Type == AmuletType.Amulet) return CardType.Power;
        return CardType.Skill; 
    }

    private static string FramePathFor(CardType frameType) =>
        ImageHelper.GetImagePath("atlases/ui_atlas.sprites/card/card_frame_" +
            frameType.ToString().ToLowerInvariant() + "_s.tres");

    private static string PortraitBorderPathFor(CardType frameType) =>
        ImageHelper.GetImagePath("atlases/ui_atlas.sprites/card/card_portrait_border_" +
            frameType.ToString().ToLowerInvariant() + "_s.tres");

    private static string AncientTextBgPathFor(CardType frameType) =>
        ImageHelper.GetImagePath("atlases/compressed.sprites/card_template/ancient_card_text_bg_" +
            frameType.ToString().ToLowerInvariant() + ".tres");

    [HarmonyPatch(typeof(CardModel), "FramePath", MethodType.Getter)]
    [HarmonyFinalizer]
    public static Exception FramePathFinalizer(CardModel __instance, Exception __exception, ref string __result)
    {
        if (__exception is ArgumentOutOfRangeException)
        {
            __result = FramePathFor(GetFrameType(__instance));
            return null;
        }
        return __exception;
    }

    [HarmonyPatch(typeof(CardModel), "PortraitBorderPath", MethodType.Getter)]
    [HarmonyFinalizer]
    public static Exception PortraitBorderPathFinalizer(CardModel __instance, Exception __exception, ref string __result)
    {
        if (__exception is ArgumentOutOfRangeException)
        {
            __result = PortraitBorderPathFor(GetFrameType(__instance));
            return null;
        }
        return __exception;
    }

    [HarmonyPatch(typeof(CardModel), "AncientTextBgPath", MethodType.Getter)]
    [HarmonyFinalizer]
    public static Exception AncientTextBgPathFinalizer(CardModel __instance, Exception __exception, ref string __result)
    {
        if (__exception is ArgumentOutOfRangeException)
        {
            __result = AncientTextBgPathFor(GetFrameType(__instance));
            return null;
        }
        return __exception;
    }

    private static string GetLocKey(CardType cardType)
    {
        if (cardType == PuppetType.Puppet) return "CARD_TYPE.PUPPET";
        if (cardType == AmuletType.Amulet) return "CARD_TYPE.AMULET";
        return "CARD_TYPE.ARTIFACT";
    }

    [HarmonyPatch(typeof(CardTypeExtensions), nameof(CardTypeExtensions.ToLocString))]
    [HarmonyFinalizer]
    public static Exception ToLocStringFinalizer(CardType cardType, Exception __exception, ref LocString __result)
    {
        if (__exception is ArgumentOutOfRangeException)
        {
            __result = new LocString("gameplay_ui", GetLocKey(cardType));
            return null;
        }
        return __exception;
    }
}
