using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class EvoRuntime
{
    public const int MaxEvoPoints = 2;
    public const int MaxSuperEvoPoints = 2;
    
    public const decimal EvolveDamageBonus = 2m;
    public const decimal EvolveBlockBonus = 2m;
    public const decimal SuperEvolveDamageBonus = 3m;
    public const decimal SuperEvolveBlockBonus = 3m;

    public enum Tier { Evolved, SuperEvolved }

    private sealed class State
    {
        public int EvoPoints;
        public int SuperEvoPoints;
        public bool UsedThisTurn;
    }

    private static readonly ConditionalWeakTable<PlayerCombatState, State> _states = new();


    private static readonly Dictionary<CardModel, Tier> _evoTier = new();
    
    public static event Action<PlayerCombatState>? Changed;

    public static void InitForCombat(PlayerCombatState pcs)
    {
        var s = Get(pcs);
        s.EvoPoints = MaxEvoPoints;
        s.SuperEvoPoints = MaxSuperEvoPoints;
        s.UsedThisTurn = false;
        _evoTier.Clear();
        Changed?.Invoke(pcs);
    }

    public static Tier? GetTier(CardModel card) =>
        _evoTier.TryGetValue(card, out var tier) ? tier : null;

    public static bool IsEvolved(CardModel card) =>
        _evoTier.TryGetValue(card, out var tier) && tier == Tier.Evolved;

    public static bool IsSuperEvolved(CardModel card) =>
        _evoTier.TryGetValue(card, out var tier) && tier == Tier.SuperEvolved;

    public static void MarkEvolved(CardModel card)
    {
        _evoTier[card] = Tier.Evolved;
        BumpSkyboundArtCounter(card);
    }

    public static void MarkSuperEvolved(CardModel card)
    {
        _evoTier[card] = Tier.SuperEvolved;
        BumpSkyboundArtCounter(card);
    }

    public static void ClearTier(CardModel card) => _evoTier.Remove(card);

    // Every evolution — regular, super, or force — increments the global
    // Skybound Art counter by 1 and refreshes the displayed counter on every
    // Skybound Art card in the player's hand. Revert-after-play does NOT
    // decrement.
    private static void BumpSkyboundArtCounter(CardModel card)
    {
        SkyboundArtRuntime.AddGlobalBonus(card.CombatState, 1);
        if (card.Owner != null)
        {
            SkyboundArtHelper.RefreshAllInHand(card.Owner);
        }
    }

    public static int EvoPoints(PlayerCombatState pcs) => Get(pcs).EvoPoints;

    public static int SuperEvoPoints(PlayerCombatState pcs) => Get(pcs).SuperEvoPoints;

    public static bool UsedThisTurn(PlayerCombatState pcs) => Get(pcs).UsedThisTurn;

    public static bool CanEvolve(PlayerCombatState pcs)
    {
        var s = Get(pcs);
        return s.EvoPoints > 0 && !s.UsedThisTurn;
    }

    public static bool CanSuperEvolve(PlayerCombatState pcs)
    {
        var s = Get(pcs);
        return s.SuperEvoPoints > 0 && !s.UsedThisTurn;
    }

    public static bool TrySpendEvo(PlayerCombatState pcs)
    {
        var s = Get(pcs);
        if (s.EvoPoints <= 0 || s.UsedThisTurn) return false;
        s.EvoPoints--;
        s.UsedThisTurn = true;
        Changed?.Invoke(pcs);
        return true;
    }

    public static bool TrySpendSuperEvo(PlayerCombatState pcs)
    {
        var s = Get(pcs);
        if (s.SuperEvoPoints <= 0 || s.UsedThisTurn) return false;
        s.SuperEvoPoints--;
        s.UsedThisTurn = true;
        Changed?.Invoke(pcs);
        return true;
    }

    public static void ResetTurnLockout(PlayerCombatState pcs)
    {
        var s = Get(pcs);
        if (!s.UsedThisTurn) return;
        s.UsedThisTurn = false;
        Changed?.Invoke(pcs);
    }

    private static State Get(PlayerCombatState pcs) => _states.GetValue(pcs, _ => new State());
}
