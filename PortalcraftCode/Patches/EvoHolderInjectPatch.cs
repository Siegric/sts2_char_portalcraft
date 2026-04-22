using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using sts2_char_portalcraft.PortalcraftCode.UI;

namespace sts2_char_portalcraft.PortalcraftCode.Patches;

[HarmonyPatch(typeof(NCombatRoom), "_Ready")]
public static class EvoHolderInjectPatch
{
    [HarmonyPostfix]
    public static void Postfix(NCombatRoom __instance)
    {
        MainFile.Logger.Info("[Evo] NCombatRoom._Ready postfix fired");

        var state = RunManager.Instance?.DebugOnlyGetState();
        if (state == null)
        {
            MainFile.Logger.Warn("[Evo] RunState null at NCombatRoom._Ready — skipping injection");
            return;
        }

        var player = LocalContext.GetMe(state);
        if (player == null)
        {
            MainFile.Logger.Warn("[Evo] LocalContext.GetMe returned null — skipping injection");
            return;
        }

        string charId = player.Character?.Id.Entry ?? "<null>";
        MainFile.Logger.Info($"[Evo] Local player character: {charId}");

        if (player.Character is not Character.sts2_char_portalcraft)
        {
            MainFile.Logger.Info("[Evo] Not Portalcraft — skipping injection");
            return;
        }
        
        const float holderSize = 128f;                    
        const float rowBottomFromScreenBottom = 200f;   
        const float evoLeftFromScreenLeft = 60f;      
        const float superEvoLeftFromScreenLeft = 180f;   

        var evoHolder = NEvoHolder.Create(isSuperEvolve: false);
        evoHolder.Name = "EvoHolder";
        AnchorBottomLeft(evoHolder, evoLeftFromScreenLeft, rowBottomFromScreenBottom, holderSize);

        var superEvoHolder = NEvoHolder.Create(isSuperEvolve: true);
        superEvoHolder.Name = "SuperEvoHolder";
        AnchorBottomLeft(superEvoHolder, superEvoLeftFromScreenLeft, rowBottomFromScreenBottom, holderSize);

        __instance.AddChildSafely(evoHolder);
        __instance.AddChildSafely(superEvoHolder);
        MainFile.Logger.Info($"[Evo] Injected 2 holders into NCombatRoom (children={__instance.GetChildCount()})");
    }
    private static void AnchorBottomLeft(Control control, float leftOffset, float bottomOffset, float size)
    {
        control.AnchorLeft = 0f;
        control.AnchorRight = 0f;
        control.AnchorTop = 1f;
        control.AnchorBottom = 1f;
        control.OffsetLeft = leftOffset;
        control.OffsetRight = leftOffset + size;
        control.OffsetTop = -(bottomOffset + size);
        control.OffsetBottom = -bottomOffset;
    }
}
