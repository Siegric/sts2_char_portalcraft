using System;
using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Runs;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.UI;

// Factory for evo holder buttons. We deliberately avoid subclassing Godot.Control in
// the mod assembly — the source-generated InvokeGodotClassMethod / HasGodotClassMethod
// bridge interacts badly with MonoMod's JIT hook for mod-loaded types, producing an
// ArgumentException at AddChildSafely time. Instead we build a vanilla Control with
// children and hook its GuiInput signal.
//
// The holder's icon swaps per stage — one texture per "remaining points" value. Drop
// PNGs named e.g. evo_icon_2.png (full), evo_icon_1.png, evo_icon_0.png (spent) and
// the holder picks the right one based on current EvoPoints / SuperEvoPoints. If a
// stage-specific file is missing, the holder falls back to the single-file icon.
public static class NEvoHolder
{
    private const float HolderSize = 64f;
    private const float IconOvershoot = 20f;

    // Per-holder state, keyed by the root Control's Godot instance id.
    private sealed class State
    {
        public bool IsSuperEvolve;
        public TextureRect IconRect = null!;
        public Texture2D?[] StageTextures = null!;  // indexed by remaining points (0..max)
        public Action<PlayerCombatState> ChangedHandler = null!;
    }

    private static readonly Dictionary<ulong, State> _states = new();

    public static Control Create(bool isSuperEvolve)
    {
        var root = new Control
        {
            CustomMinimumSize = new Vector2(HolderSize, HolderSize),
            Size = new Vector2(HolderSize, HolderSize),
            MouseFilter = Control.MouseFilterEnum.Stop,
        };

        var stageTextures = LoadStageTextures(isSuperEvolve);

        // Icon extends beyond the holder's click area so the visible graphic is
        // larger than the hit box — easier to read at a glance.
        var icon = new TextureRect
        {
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspect,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            AnchorRight = 1f,
            AnchorBottom = 1f,
            OffsetLeft = -IconOvershoot,
            OffsetTop = -IconOvershoot,
            OffsetRight = IconOvershoot,
            OffsetBottom = IconOvershoot,
        };
        root.AddChild(icon);

        // Register state + event subscriptions.
        var id = root.GetInstanceId();
        var state = new State
        {
            IsSuperEvolve = isSuperEvolve,
            IconRect = icon,
            StageTextures = stageTextures,
        };

        var rootRef = root;
        state.ChangedHandler = _ => UpdateDisplay(rootRef);
        EvoRuntime.Changed += state.ChangedHandler;

        _states[id] = state;

        // Periodic refresh — hand contents change without firing EvoRuntime.Changed
        // (cards drawn/discarded), so we poll to keep the greyed-out state in sync.
        var refreshTimer = new Godot.Timer
        {
            WaitTime = 0.2,
            Autostart = true,
            OneShot = false,
        };
        root.AddChild(refreshTimer);
        refreshTimer.Connect(Godot.Timer.SignalName.Timeout,
            Callable.From(() => UpdateDisplay(rootRef)));

        // Click handler.
        root.Connect(Control.SignalName.GuiInput,
            Callable.From<InputEvent>(evt => OnGuiInput(rootRef, evt)));

        // Cleanup when the holder leaves the tree (room change, combat end).
        root.Connect(Node.SignalName.TreeExiting,
            Callable.From(() => OnTreeExiting(id)));

        UpdateDisplay(root);
        return root;
    }

    private static Texture2D?[] LoadStageTextures(bool isSuperEvolve)
    {
        string prefix = isSuperEvolve ? "super_evo_icon" : "evo_icon";
        int max = isSuperEvolve ? EvoRuntime.MaxSuperEvoPoints : EvoRuntime.MaxEvoPoints;

        string fallbackPath = $"res://sts2_char_portalcraft/images/charui/{prefix}.png";
        Texture2D? fallback = ResourceLoader.Exists(fallbackPath)
            ? ResourceLoader.Load<Texture2D>(fallbackPath)
            : null;

        var arr = new Texture2D?[max + 1];
        for (int i = 0; i <= max; i++)
        {
            string stagePath = $"res://sts2_char_portalcraft/images/charui/{prefix}_{i}.png";
            arr[i] = ResourceLoader.Exists(stagePath)
                ? ResourceLoader.Load<Texture2D>(stagePath)
                : fallback;
        }
        return arr;
    }

    private static void OnGuiInput(Control root, InputEvent evt)
    {
        if (!_states.TryGetValue(root.GetInstanceId(), out var state)) return;
        if (evt is not InputEventMouseButton mb) return;
        if (mb.ButtonIndex != MouseButton.Left) return;
        // Trigger on press so the arrow appears while the mouse button is still held;
        // NTargetManager in ReleaseMouseToTarget mode completes on the subsequent release.
        if (!mb.Pressed) return;

        root.GetViewport().SetInputAsHandled();
        TriggerEvolve(root, state);
    }

    private static void OnTreeExiting(ulong id)
    {
        if (!_states.TryGetValue(id, out var state)) return;
        EvoRuntime.Changed -= state.ChangedHandler;
        _states.Remove(id);
    }

    private static void UpdateDisplay(Control root)
    {
        if (!_states.TryGetValue(root.GetInstanceId(), out var state)) return;

        int max = state.IsSuperEvolve ? EvoRuntime.MaxSuperEvoPoints : EvoRuntime.MaxEvoPoints;
        var player = GetLocalPlayer();

        // Pre-combat (no PlayerCombatState) preview shows the "full" stage.
        int count = player?.PlayerCombatState == null
            ? max
            : state.IsSuperEvolve
                ? EvoRuntime.SuperEvoPoints(player.PlayerCombatState)
                : EvoRuntime.EvoPoints(player.PlayerCombatState);

        int clamped = Math.Clamp(count, 0, state.StageTextures.Length - 1);
        var tex = state.StageTextures[clamped];
        if (tex != null && state.IconRect.Texture != tex)
        {
            state.IconRect.Texture = tex;
        }

        if (player == null)
        {
            root.Modulate = new Color(0.6f, 0.6f, 0.6f, 1f);
            return;
        }

        bool canUse = state.IsSuperEvolve
            ? EvoCmd.CanSuperEvolveAny(player)
            : EvoCmd.CanEvolveAny(player);
        root.Modulate = canUse
            ? new Color(1f, 1f, 1f, 1f)
            : new Color(0.5f, 0.5f, 0.5f, 0.9f);
    }

    private static void TriggerEvolve(Control root, State state)
    {
        var player = GetLocalPlayer();
        if (player == null) return;

        bool canUse = state.IsSuperEvolve
            ? EvoCmd.CanSuperEvolveAny(player)
            : EvoCmd.CanEvolveAny(player);
        if (!canUse) return;

        var arrowStart = root.GlobalPosition + root.Size * 0.5f;
        var ctx = new BlockingPlayerChoiceContext();

        TaskHelper.RunSafely(state.IsSuperEvolve
            ? EvoCmd.SuperEvolveFromHandWithArrow(player, ctx, arrowStart)
            : EvoCmd.EvolveFromHandWithArrow(player, ctx, arrowStart));
    }

    private static Player? GetLocalPlayer()
    {
        var state = RunManager.Instance?.DebugOnlyGetState();
        if (state == null) return null;
        return LocalContext.GetMe(state);
    }
}
