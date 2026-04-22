using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class FuseRuntime
{
    private static readonly HashSet<CardModel> _active = new();

    public static void Mark(CardModel card) => _active.Add(card);

    public static bool TryConsume(CardModel card) => _active.Remove(card);

    public static bool IsActive(CardModel card) => _active.Contains(card);
}
