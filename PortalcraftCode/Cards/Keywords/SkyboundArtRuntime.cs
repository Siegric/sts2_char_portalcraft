using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class SkyboundArtRuntime
{
    private sealed class Counter { public int Value; }

    private static readonly ConditionalWeakTable<CardModel, Counter> _bonus = new();

    public static int GetBonus(CardModel card) =>
        _bonus.TryGetValue(card, out var c) ? c.Value : 0;

    public static void AddBonus(CardModel card, int amount)
    {
        var counter = _bonus.GetValue(card, _ => new Counter());
        counter.Value += amount;
    }

    public static void Clear(CardModel card) => _bonus.Remove(card);
}
