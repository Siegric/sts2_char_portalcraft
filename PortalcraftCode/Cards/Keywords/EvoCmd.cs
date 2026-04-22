using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

// Gateway commands for applying Evolution / Super Evolution to a card.
public static class EvoCmd
{
    // --- Eligibility predicates ---------------------------------------------
    //
    // Player-initiated (UI): points, turn lockout, and tier all enforced.
    public static bool CanEvolve(CardModel card)
    {
        if (!CanForceEvolve(card)) return false;
        return EvoRuntime.CanEvolve(card.Owner!.PlayerCombatState!);
    }

    public static bool CanSuperEvolve(CardModel card)
    {
        if (!CanForceSuperEvolve(card)) return false;
        return EvoRuntime.CanSuperEvolve(card.Owner!.PlayerCombatState!);
    }

    // Card-initiated (Force*): tier-only. Skips EP/SEP cost + turn lockout so
    // card effects (OnPlay, OnEvolve, etc.) aren't gated by the player's pool.
    // Intended use: filters for CardSelectCmd when a card effect picks an evolve target.
    public static bool CanForceEvolve(CardModel card)
    {
        if (card is not IEvolvableCard) return false;
        if (card.Owner?.PlayerCombatState == null) return false;
        // Already at some tier → can't re-evolve.
        return EvoRuntime.GetTier(card) == null;
    }

    public static bool CanForceSuperEvolve(CardModel card)
    {
        if (card is not IEvolvableCard) return false;
        if (card.Owner?.PlayerCombatState == null) return false;
        // Already super → can't re-super-evolve.
        return EvoRuntime.GetTier(card) != EvoRuntime.Tier.SuperEvolved;
    }

    // --- Player-initiated entry points (spend EP/SEP) -----------------------
    public static async Task<bool> TryEvolve(CardModel card, PlayerChoiceContext choiceContext)
    {
        if (!CanEvolve(card)) return false;
        if (!EvoRuntime.TrySpendEvo(card.Owner!.PlayerCombatState!)) return false;
        return await DoEvolveInternal(card, choiceContext, playVfx: true);
    }

    public static async Task<bool> TrySuperEvolve(CardModel card, PlayerChoiceContext choiceContext)
    {
        if (!CanSuperEvolve(card)) return false;
        if (!EvoRuntime.TrySpendSuperEvo(card.Owner!.PlayerCombatState!)) return false;
        return await DoSuperEvolveInternal(card, choiceContext, playVfx: true);
    }

    // --- Card-initiated entry points (no EP/SEP cost) -----------------------
    // Use from card effects: self-evolve on play, evolve a target card, etc.
    // playVfx=false is handy when the card-play animation already provides
    // visual feedback and you don't want the 0.5s / 3s evolve VFX on top of it.
    public static async Task<bool> ForceEvolve(CardModel card, PlayerChoiceContext choiceContext, bool playVfx = true)
    {
        if (!CanForceEvolve(card)) return false;
        return await DoEvolveInternal(card, choiceContext, playVfx);
    }

    public static async Task<bool> ForceSuperEvolve(CardModel card, PlayerChoiceContext choiceContext, bool playVfx = true)
    {
        if (!CanForceSuperEvolve(card)) return false;
        return await DoSuperEvolveInternal(card, choiceContext, playVfx);
    }

    // --- Shared core pipeline -----------------------------------------------
    // Mark tier → refresh glow → (optional) VFX → invoke card's OnEvolve hook.
    // Preconditions (eligibility + point-spend) are the caller's responsibility.
    private static async Task<bool> DoEvolveInternal(CardModel card, PlayerChoiceContext ctx, bool playVfx)
    {
        if (card is not IEvolvableCard evolvable) return false;
        EvoRuntime.MarkEvolved(card);
        RefreshHandCardGlow(card);
        if (playVfx) await PlayEvolveVfx(card);
        await evolvable.OnEvolve(card, ctx);
        return true;
    }

    private static async Task<bool> DoSuperEvolveInternal(CardModel card, PlayerChoiceContext ctx, bool playVfx)
    {
        if (card is not IEvolvableCard evolvable) return false;
        EvoRuntime.MarkSuperEvolved(card);
        RefreshHandCardGlow(card);
        if (playVfx) await PlaySuperEvolveVfx(card);
        await evolvable.OnSuperEvolve(card, ctx);
        return true;
    }

    // Quick in-hand scale animation for regular evolve — keeps the card in place,
    // scales up 1.5× and back. ~0.5s total. Uses the game's built-in transform helper
    // with startCard == endCard so the Model swap is a no-op; we just ride the animation.
    private static async Task PlayEvolveVfx(CardModel card)
    {
        var nCard = NCard.FindOnTable(card);
        if (nCard == null) return;
        await NCardTransformVfx.PlayAnimOnCardInHand(nCard, card);
    }

    // Full center-screen VFX for super evolve — the card flies to center, flashes +
    // boings. We terminate the VFX before its built-in fly-back runs, because that
    // fly-back targets PileType.Hand's entry point (below the screen where new cards
    // arrive) rather than the card's actual hand slot; letting it run makes the card
    // yeet off-screen for a moment before popping back into place.
    //
    // The VFX is parented to CardPreviewContainer (centered on screen). Parenting to
    // CombatVfxContainer would render at the combat room's origin (top-right / corner).
    //
    // PreBackflightDurationSec covers the initial scale, brightness flash, and boing
    // wiggle sequence. Stop here and the scale-up / flash finale stay intact.
    private const double PreBackflightDurationSec = 2.6;
    private const double BackflightDurationSec = 0.35;

    private static async Task PlaySuperEvolveVfx(CardModel card)
    {
        var nCard = NCard.FindOnTable(card);
        if (nCard == null) return;

        var vfx = NCardTransformVfx.Create(card, card, null);
        if (vfx == null) return;  // TestMode.IsOn — skip visually

        var previewContainer = NCombatRoom.Instance?.Ui.CardPreviewContainer
                               ?? NRun.Instance?.GlobalUi.CardPreviewContainer;
        if (previewContainer == null)
        {
            vfx.QueueFreeSafely();
            return;
        }

        bool prevVisible = nCard.Visible;
        nCard.Visible = false;
        try
        {
            previewContainer.AddChildSafely(vfx);

            var tree = nCard.GetTree();
            if (tree != null)
            {
                // Phase 1: wait for the center-stage flash/boing to finish.
                var flashTimer = tree.CreateTimer(PreBackflightDurationSec);
                await nCard.ToSignal(flashTimer, SceneTreeTimer.SignalName.Timeout);

                // Phase 2: smooth slide from center back to the card's real hand
                // slot, fading out as it arrives. Compute the target as the current
                // local position plus the global-space delta to the hand card, so
                // we don't have to worry about preview-container transforms.
                if (GodotObject.IsInstanceValid(vfx) && GodotObject.IsInstanceValid(nCard))
                {
                    var delta = nCard.GlobalPosition - vfx.GlobalPosition;
                    var targetLocalPos = vfx.Position + delta;
                    var flyTween = vfx.CreateTween().SetParallel(true);
                    flyTween.TweenProperty(vfx, "position", targetLocalPos, BackflightDurationSec)
                        .SetEase(Tween.EaseType.InOut)
                        .SetTrans(Tween.TransitionType.Cubic);
                    flyTween.TweenProperty(vfx, "modulate:a", 0.0, BackflightDurationSec)
                        .SetEase(Tween.EaseType.In);
                    await nCard.ToSignal(flyTween, Tween.SignalName.Finished);
                }
            }
        }
        finally
        {
            if (GodotObject.IsInstanceValid(vfx))
            {
                vfx.QueueFreeSafely();
            }
            if (GodotObject.IsInstanceValid(nCard))
            {
                nCard.Visible = prevVisible;
            }
        }
    }

    // NHandCardHolder only re-evaluates ShouldGlowGold inside UpdateCard(), which the
    // game only calls at draw / SetCard time. After an in-hand mutation like evolve,
    // we have to poke it manually or the glow won't appear until the card is re-drawn.
    private static void RefreshHandCardGlow(CardModel card)
    {
        var hand = NPlayerHand.Instance;
        if (hand == null) return;
        var holder = hand.ActiveHolders?.FirstOrDefault(h => h.CardNode?.Model == card);
        holder?.UpdateCard();
    }

    // Player-scoped eligibility: does the player have the resource AND any evolvable target in hand?
    // Used by UI to enable/disable the evo holders.
    public static bool CanEvolveAny(Player player)
    {
        if (player.PlayerCombatState == null) return false;
        if (!EvoRuntime.CanEvolve(player.PlayerCombatState)) return false;
        return PileType.Hand.GetPile(player).Cards.Any(CanEvolve);
    }

    public static bool CanSuperEvolveAny(Player player)
    {
        if (player.PlayerCombatState == null) return false;
        if (!EvoRuntime.CanSuperEvolve(player.PlayerCombatState)) return false;
        return PileType.Hand.GetPile(player).Cards.Any(CanSuperEvolve);
    }

    // Full evolve flow: opens the hand-card-selection UI, waits for the player's pick,
    // then applies TryEvolve. Returns false if eligibility fails or the player cancels.
    // Caller provides a PlayerChoiceContext (typically via a GameAction wrapper from the UI).
    public static async Task<bool> EvolveFromHand(Player player, PlayerChoiceContext choiceContext)
    {
        if (!CanEvolveAny(player)) return false;
        var selected = await SelectEvolveTarget(player, superEvolve: false);
        if (selected == null) return false;
        return await TryEvolve(selected, choiceContext);
    }

    public static async Task<bool> SuperEvolveFromHand(Player player, PlayerChoiceContext choiceContext)
    {
        if (!CanSuperEvolveAny(player)) return false;
        var selected = await SelectEvolveTarget(player, superEvolve: true);
        if (selected == null) return false;
        return await TrySuperEvolve(selected, choiceContext);
    }

    private static async Task<CardModel?> SelectEvolveTarget(Player player, bool superEvolve)
    {
        var combatRoom = NCombatRoom.Instance;
        if (combatRoom == null) return null;

        NPlayerHand.Instance?.CancelAllCardPlay();

        string headerKey = superEvolve ? "SUPER_EVOLVE_SELECT_HEADER" : "EVOLVE_SELECT_HEADER";
        var prefs = new CardSelectorPrefs(new LocString("gameplay_ui", headerKey), 1);
        Func<CardModel, bool> filter = superEvolve ? CanSuperEvolve : CanEvolve;

        var result = await combatRoom.Ui.Hand.SelectCards(
            prefs, filter, source: null, NPlayerHand.Mode.SimpleSelect);
        return result.FirstOrDefault();
    }

    // Arrow-based evolve flow (alternate to EvolveFromHand). Uses NTargetManager with a
    // custom session flag (EvoTargeting) so NCard's MouseEntered hooks participate only
    // during this selection. The caller supplies the arrow origin (e.g. the evo holder's
    // global position). Returns false on eligibility failure or player cancel.
    public static async Task<bool> EvolveFromHandWithArrow(Player player, PlayerChoiceContext choiceContext, Vector2 arrowStart)
    {
        if (!CanEvolveAny(player)) return false;
        var selected = await SelectWithArrow(superEvolve: false, arrowStart);
        if (selected == null) return false;
        return await TryEvolve(selected, choiceContext);
    }

    public static async Task<bool> SuperEvolveFromHandWithArrow(Player player, PlayerChoiceContext choiceContext, Vector2 arrowStart)
    {
        if (!CanSuperEvolveAny(player)) return false;
        var selected = await SelectWithArrow(superEvolve: true, arrowStart);
        if (selected == null) return false;
        return await TrySuperEvolve(selected, choiceContext);
    }

    private static async Task<CardModel?> SelectWithArrow(bool superEvolve, Vector2 arrowStart)
    {
        var tm = NTargetManager.Instance;
        if (tm == null) return null;

        NPlayerHand.Instance?.CancelAllCardPlay();
        EvoTargeting.Begin(superEvolve);
        try
        {
            Func<Node, bool> nodeFilter = node =>
                node is NCard nCard &&
                nCard.Model != null &&
                EvoTargeting.IsEvolvable(nCard.Model);

            // ReleaseMouseToTarget: arrow appears immediately on press (caller already
            // ate the press event), tracks the mouse while held, and NTargetManager
            // selects whatever valid target is hovered when the user releases the button.
            tm.StartTargeting(
                TargetType.TargetedNoCreature,
                arrowStart,
                TargetMode.ReleaseMouseToTarget,
                exitEarlyCondition: null,
                nodeFilter: nodeFilter);

            var node = await tm.SelectionFinished();
            return (node as NCard)?.Model;
        }
        finally
        {
            EvoTargeting.End();
        }
    }
}
