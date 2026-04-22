using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Patches;

public static class NCardEvoTargetingPatch
{
    [HarmonyPatch(typeof(NHandCardHolder), "OnFocus")]
    public static class OnFocusPatch
    {
        [HarmonyPrefix]
        public static void Prefix(NHandCardHolder __instance)
        {
            if (!EvoTargeting.IsActive) return;
            var tm = NTargetManager.Instance;
            if (tm == null || !tm.IsInSelection) return;
            var nCard = __instance.CardNode;
            if (nCard == null) return;
            if (!tm.AllowedToTargetNode(nCard)) return;
            tm.OnNodeHovered(nCard);
        }
    }

    [HarmonyPatch(typeof(NHandCardHolder), "OnUnfocus")]
    public static class OnUnfocusPatch
    {
        [HarmonyPrefix]
        public static void Prefix(NHandCardHolder __instance)
        {
            if (!EvoTargeting.IsActive) return;
            var tm = NTargetManager.Instance;
            if (tm == null) return;
            var nCard = __instance.CardNode;
            if (nCard == null) return;
            tm.OnNodeUnhovered(nCard);
        }
    }
    
    [HarmonyPatch(typeof(NPlayerHand), "StartCardPlay")]
    public static class StartCardPlayPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return !EvoTargeting.IsActive;
        }
    }
    
    [HarmonyPatch(typeof(NCardHolder), "CreateHoverTips")]
    public static class CreateHoverTipsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return !EvoTargeting.IsActive;
        }
    }
}
