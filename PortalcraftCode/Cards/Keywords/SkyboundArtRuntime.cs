using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class SkyboundArtRuntime
{
    // Adjust Thrresholds here
    public const int SkyboundArtThreshold = 10;
    public const int SuperSkyboundArtThreshold = 15;

    private sealed class Counter { public int Value; }
    private static readonly ConditionalWeakTable<CombatState, Counter> _globalBonus = new();

    public static int GlobalBonus(CombatState? combatState) =>
        combatState != null && _globalBonus.TryGetValue(combatState, out var c) ? c.Value : 0;

    public static void AddGlobalBonus(CombatState? combatState, int amount)
    {
        if (combatState == null) return;
        var counter = _globalBonus.GetValue(combatState, _ => new Counter());
        counter.Value += amount;
    }

    public static int CurrentGauge(CardModel card)
    {
        int round = card.CombatState?.RoundNumber ?? 0;
        return round + GlobalBonus(card.CombatState);
    }

    public static bool IsActive(CardModel card) =>
        CurrentGauge(card) >= SkyboundArtThreshold;

    public static bool IsSuperActive(CardModel card) =>
        CurrentGauge(card) >= SuperSkyboundArtThreshold;

    private static readonly ConditionalWeakTable<CardModel, object> _firedSa = new();
    private static readonly ConditionalWeakTable<CardModel, object> _firedSuperSa = new();

    public static bool HasFiredSkyboundArt(CardModel card) => _firedSa.TryGetValue(card, out _);
    public static void MarkSkyboundArtFired(CardModel card)
    {
        if (!_firedSa.TryGetValue(card, out _)) _firedSa.Add(card, new object());
    }

    public static bool HasFiredSuperSkyboundArt(CardModel card) => _firedSuperSa.TryGetValue(card, out _);
    public static void MarkSuperSkyboundArtFired(CardModel card)
    {
        if (!_firedSuperSa.TryGetValue(card, out _)) _firedSuperSa.Add(card, new object());
    }
}
