using System;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Patches;

public static class CrystallizePatches
{
    [HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
    public static class CrystallizeAffordabilityPatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            CardModel card,
            PlayerCombatState __instance,
            ref bool __result,
            ref UnplayableReason reason)
        {
            if (__result) return;
            if (card is not ICrystallizeCard cc) return;
            if ((reason & UnplayableReason.EnergyCostTooHigh) == 0) return;
            if (__instance.Energy < cc.CrystallizeCost) return;

            reason &= ~UnplayableReason.EnergyCostTooHigh;
            if (reason == UnplayableReason.None) __result = true;
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
    public static class CrystallizeSpendResourcesPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CardModel __instance, ref Task<(int, int)> __result)
        {
            if (__instance is not ICrystallizeCard cc) return true;
            if (__instance.Owner?.PlayerCombatState == null) return true;

            int fullCost = Math.Max(0, __instance.EnergyCost.GetWithModifiers(CostModifiers.All));
            int energy = __instance.Owner.PlayerCombatState.Energy;
            if (energy >= fullCost) return true;

            int crystallizeCost = Math.Max(0, cc.CrystallizeCost);
            if (energy < crystallizeCost) return true;

            CrystallizeRuntime.Mark(__instance);
            __result = SpendCrystallizeAsync(__instance, crystallizeCost);
            return false;
        }

        private static async Task<(int, int)> SpendCrystallizeAsync(CardModel card, int crystallizeCost)
        {
            if (crystallizeCost > 0)
            {
                CombatManager.Instance.History.EnergySpent(card.CombatState, crystallizeCost, card.Owner);
                card.Owner.PlayerCombatState.LoseEnergy(crystallizeCost);
            }
            await Hook.AfterEnergySpent(card.CombatState, card, crystallizeCost);
            return (crystallizeCost, 0);
        }
    }
}
