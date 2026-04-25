using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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
using sts2_char_portalcraft.PortalcraftCode.Audio;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class EvoCmd
{
    public static bool CanEvolve(CardModel card)
    {
        if (!CanForceEvolve(card)) return false;
        return EvoRuntime.CanEvolve(card.Owner!);
    }

    public static bool CanSuperEvolve(CardModel card)
    {
        if (!CanForceSuperEvolve(card)) return false;
        return EvoRuntime.CanSuperEvolve(card.Owner!);
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
        return await DoEvolveInternal(card, choiceContext, playVfx: true, spendPoint: true);
    }

    public static async Task<bool> TrySuperEvolve(CardModel card, PlayerChoiceContext choiceContext)
    {
        if (!CanSuperEvolve(card)) return false;
        return await DoSuperEvolveInternal(card, choiceContext, playVfx: true, spendPoint: true);
    }

    public static async Task<bool> ForceEvolve(CardModel card, PlayerChoiceContext choiceContext, bool playVfx = true)
    {
        if (!CanForceEvolve(card)) return false;
        return await DoEvolveInternal(card, choiceContext, playVfx, spendPoint: false);
    }

    public static async Task<bool> ForceSuperEvolve(CardModel card, PlayerChoiceContext choiceContext, bool playVfx = true)
    {
        if (!CanForceSuperEvolve(card)) return false;
        return await DoSuperEvolveInternal(card, choiceContext, playVfx, spendPoint: false);
    }
    
    private static async Task<bool> DoEvolveInternal(CardModel card, PlayerChoiceContext ctx, bool playVfx, bool spendPoint)
    {
        if (card is not IEvolvableCard evolvable) return false;

        CardModel finalCard = card;
        if (evolvable.EvolvedType is { } newType)
        {
            var created = CreateTransformedCard(card, newType);
            if (created == null) return false;
            if (spendPoint && !await EvoRuntime.TrySpendEvo(card.Owner!, ctx)) return false;
            // Set tier on the new mutable card BEFORE it is added via the networked
            // AddGeneratedCardToCombat. Since sts2_char_portalcraft_CurrentTier is a SavedProperty it rides
            // the card's serialization to all clients, so both ends agree on the tier.
            if (created is IEvolvableCard createdEv) createdEv.sts2_char_portalcraft_CurrentTier = EvoTier.Evolved;
            await ReplaceInHand(card, created, ctx);
            finalCard = created;
        }
        else if (spendPoint && !await EvoRuntime.TrySpendEvo(card.Owner!, ctx))
        {
            return false;
        }

        await EvoRuntime.MarkEvolved(finalCard, ctx);
        RefreshHandCardGlow(finalCard);
        if (playVfx)
        {
            CardPlayAudioManager.PlayForEvolve(card.GetType().Name);
            await PlayEvolveVfx(finalCard);
        }

        var hookTarget = finalCard as IEvolvableCard ?? evolvable;
        await hookTarget.OnEvolve(finalCard, ctx);
        return true;
    }

    private static async Task<bool> DoSuperEvolveInternal(CardModel card, PlayerChoiceContext ctx, bool playVfx, bool spendPoint)
    {
        if (card is not IEvolvableCard evolvable) return false;

        CardModel finalCard = card;
        if (evolvable.SuperEvolvedType is { } newType)
        {
            var created = CreateTransformedCard(card, newType);
            if (created == null) return false;
            if (spendPoint && !await EvoRuntime.TrySpendSuperEvo(card.Owner!, ctx)) return false;
            if (created is IEvolvableCard createdEv) createdEv.sts2_char_portalcraft_CurrentTier = EvoTier.SuperEvolved;
            await ReplaceInHand(card, created, ctx);
            finalCard = created;
        }
        else if (spendPoint && !await EvoRuntime.TrySpendSuperEvo(card.Owner!, ctx))
        {
            return false;
        }

        await EvoRuntime.MarkSuperEvolved(finalCard, ctx);
        RefreshHandCardGlow(finalCard);
        if (playVfx)
        {
            CardPlayAudioManager.PlayForSuperEvolve(card.GetType().Name);
            await PlaySuperEvolveVfx(finalCard);
        }
        
        var hookTarget = finalCard as IEvolvableCard ?? evolvable;
        await hookTarget.OnEvolve(finalCard, ctx);
        await hookTarget.OnSuperEvolve(finalCard, ctx);
        return true;
    }
    
    private static CardModel? CreateTransformedCard(CardModel sourceCard, Type newType)
    {
        if (!typeof(CardModel).IsAssignableFrom(newType)) return null;
        var owner = sourceCard.Owner;
        var combatState = sourceCard.CombatState;
        if (owner == null || combatState == null) return null;

        var modelDbMethod = typeof(ModelDb).GetMethod(nameof(ModelDb.Card), Type.EmptyTypes);
        if (modelDbMethod == null) return null;
        if (modelDbMethod.MakeGenericMethod(newType).Invoke(null, null) is not CardModel canonical) return null;
        var newCard = combatState.CreateCard(canonical, owner);
        if (newCard == null) return null;

        int levels = sourceCard.CurrentUpgradeLevel;
        for (int i = 0; i < levels; i++) newCard.UpgradeInternal();
        return newCard;
    }
    
    private static async Task ReplaceInHand(CardModel oldCard, CardModel newCard, PlayerChoiceContext ctx)
    {
        using (Patches.CannotBeExhaustedPatch.BypassScope())
        {
            await CardCmd.Exhaust(ctx, oldCard);
        }
        await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, newCard.Owner);
    }

    public static async Task PlayEvolveVfx(CardModel card)
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
        if (!EvoRuntime.CanEvolve(player)) return false;
        return PileType.Hand.GetPile(player).Cards.Any(CanEvolve);
    }

    public static bool CanSuperEvolveAny(Player player)
    {
        if (player.PlayerCombatState == null) return false;
        if (!EvoRuntime.CanSuperEvolve(player)) return false;
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
