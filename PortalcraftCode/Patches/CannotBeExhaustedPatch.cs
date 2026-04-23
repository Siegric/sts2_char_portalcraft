using System;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Patches;

[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Exhaust))]
public static class CannotBeExhaustedPatch
{
    private static readonly AsyncLocal<int> _bypassDepth = new();
    
    public static IDisposable BypassScope()
    {
        _bypassDepth.Value++;
        return new Bypass();
    }

    private sealed class Bypass : IDisposable
    {
        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _bypassDepth.Value--;
        }
    }

    public static bool Prefix(CardModel card, ref Task __result)
    {
        if (card == null) return true;
        if (_bypassDepth.Value > 0) return true; 
        if (!card.Keywords.Contains(CannotBeExhaustedKeyword.CannotBeExhausted)) return true;

        __result = Task.CompletedTask;
        return false;  // skip the original exhaust
    }
}
