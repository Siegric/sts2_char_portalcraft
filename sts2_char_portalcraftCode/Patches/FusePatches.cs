using System;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Patches;

public static class FusePatches
{
    [HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
    public static class FuseAffordabilityPatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            CardModel card,
            PlayerCombatState __instance,
            ref bool __result,
            ref UnplayableReason reason)
        {
            if (__result) return;
            if (card is not IFuseCard fc) return;
            if ((reason & UnplayableReason.EnergyCostTooHigh) == 0) return;
            if (__instance.Energy < fc.FuseCost) return;
            if (!fc.HasValidFusionPartnerInHand()) return;

            reason &= ~UnplayableReason.EnergyCostTooHigh;
            if (reason == UnplayableReason.None) __result = true;
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
    public static class FuseSpendResourcesPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel __instance, ref Task<(int, int)> __result)
        {
            if (__instance is not IFuseCard fc) return true;
            if (__instance.Owner?.PlayerCombatState == null) return true;

            int fullCost = Math.Max(0, __instance.EnergyCost.GetWithModifiers(CostModifiers.All));
            int energy = __instance.Owner.PlayerCombatState.Energy;
            if (energy >= fullCost) return true;

            int fuseCost = Math.Max(0, fc.FuseCost);
            if (energy < fuseCost) return true;
            if (!fc.HasValidFusionPartnerInHand()) return true;

            FuseRuntime.Mark(__instance);
            __result = SpendFuseAsync(__instance, fuseCost);
            return false;
        }

        private static async Task<(int, int)> SpendFuseAsync(CardModel card, int fuseCost)
        {
            if (fuseCost > 0)
            {
                CombatManager.Instance.History.EnergySpent(card.CombatState, fuseCost, card.Owner);
                card.Owner.PlayerCombatState.LoseEnergy(fuseCost);
            }
            await Hook.AfterEnergySpent(card.CombatState, card, fuseCost);
            return (fuseCost, 0);
        }
    }
}
