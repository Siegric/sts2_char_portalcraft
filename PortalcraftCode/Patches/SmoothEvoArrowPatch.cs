#if FALSE
using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Patches;

public static class SmoothEvoArrowPatch
{
    private static readonly Color EvolveColor      = new(1.00f, 0.84f, 0.00f, 1f); // gold
    private static readonly Color SuperEvolveColor = new(0.70f, 0.40f, 1.00f, 1f); // purple

    private static ImageTexture? _evolveGradient;
    private static ImageTexture? _superEvolveGradient;

    private static ImageTexture GetGradientTex(bool isSuper)
        => isSuper
            ? _superEvolveGradient ??= MakeGradient(SuperEvolveColor)
            : _evolveGradient      ??= MakeGradient(EvolveColor);

    private static ImageTexture MakeGradient(Color edge)
    {
        const int height = 64;
        var img = Image.CreateEmpty(1, height, false, Image.Format.Rgba8);
        for (int y = 0; y < height; y++)
        {
            float v = y / (float)(height - 1);
            float dist = Math.Abs(v - 0.5f) * 2f; // 0 at center, 1 at edges
            img.SetPixel(0, y, Colors.White.Lerp(edge, dist));
        }
        return ImageTexture.CreateFromImage(img);
    }

    [HarmonyPatch(typeof(NTargetingArrow), "UpdateSegments")]
    public static class UpdateSegmentsPatch
    {
        [HarmonyPostfix]
        public static void Postfix(NTargetingArrow __instance, Vector2 initialPos, Vector2 finalPos, Vector2 controlPoint)
        {
            if (!EvoTargeting.IsActive) return;

            var segments = AccessTools.FieldRefAccess<NTargetingArrow, Sprite2D[]>("_segments")(__instance);
            if (segments != null) foreach (var s in segments) if (s != null) s.Visible = false;

            var head = AccessTools.FieldRefAccess<NTargetingArrow, Sprite2D>("_arrowHead")(__instance);
            if (head != null) head.Visible = false;

            var line = __instance.GetNodeOrNull<Line2D>("EvoSmoothLine") ?? CreateLine(__instance);

            const int pointCount = 30;
            var pts = new Vector2[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                float t = i / (float)(pointCount - 1);
                pts[i] = MathHelper.BezierCurve(initialPos, finalPos, controlPoint, t);
            }

            line.Texture = GetGradientTex(EvoTargeting.IsSuperEvolveMode);
            line.TextureMode = Line2D.LineTextureMode.Stretch;
            line.DefaultColor = Colors.White;
            line.Points = pts;
            line.Visible = true;
        }

        private static Line2D CreateLine(NTargetingArrow parent)
        {
            var line = new Line2D
            {
                Name = "EvoSmoothLine",
                Width = 12.0f,
                DefaultColor = Colors.White,
                JointMode = Line2D.LineJointMode.Round,
                BeginCapMode = Line2D.LineCapMode.Round,
                EndCapMode = Line2D.LineCapMode.Round,
                ZIndex = -1,
            };
            parent.AddChild(line);
            return line;
        }
    }

    [HarmonyPatch(typeof(NTargetingArrow), nameof(NTargetingArrow.StopDrawing))]
    public static class StopDrawingPatch
    {
        [HarmonyPostfix]
        public static void Postfix(NTargetingArrow __instance)
        {
            var line = __instance.GetNodeOrNull<Line2D>("EvoSmoothLine");
            if (line != null) line.Visible = false;

            var segments = AccessTools.FieldRefAccess<NTargetingArrow, Sprite2D[]>("_segments")(__instance);
            if (segments != null) foreach (var s in segments) if (s != null) s.Visible = true;

            var head = AccessTools.FieldRefAccess<NTargetingArrow, Sprite2D>("_arrowHead")(__instance);
            if (head != null) head.Visible = true;
        }
    }

    [HarmonyPatch(typeof(NTargetingArrow), nameof(NTargetingArrow.SetHighlightingOn))]
    public static class SuppressModulatePatch
    {
        [HarmonyPostfix]
        public static void Postfix(NTargetingArrow __instance)
        {
            if (!EvoTargeting.IsActive) return;
            __instance.Modulate = Colors.White;
        }
    }
}
#endif