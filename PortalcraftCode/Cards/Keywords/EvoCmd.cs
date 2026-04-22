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

public static class EvoCmd
{
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
        return EvoRuntime.GetTier(card) == null;
    }
    
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
    
    private static async Task PlayEvolveVfx(CardModel card)
    {
        var nCard = NCard.FindOnTable(card);
        if (nCard == null) return;
        await NCardTransformVfx.PlayAnimOnCardInHand(nCard, card);
    }
    
    private const double PreBackflightDurationSec = 1.3;
    private const double BackflightDurationSec = 0.35;

    private static async Task PlaySuperEvolveVfx(CardModel card)
    {
        var nCard = NCard.FindOnTable(card);
        if (nCard == null) return;

        var vfx = NCardTransformVfx.Create(card, card, null);
        if (vfx == null) return;  // TestMode.IsOn

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
                var flashTimer = tree.CreateTimer(PreBackflightDurationSec);
                await nCard.ToSignal(flashTimer, SceneTreeTimer.SignalName.Timeout);
                
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
    
    private static void RefreshHandCardGlow(CardModel card)
    {
        var hand = NPlayerHand.Instance;
        if (hand == null) return;
        var holder = hand.ActiveHolders?.FirstOrDefault(h => h.CardNode?.Model == card);
        holder?.UpdateCard();
    }
    
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
