using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class EvoTargeting
{
    public static bool IsActive { get; private set; }

    public static bool IsSuperEvolveMode { get; private set; }

    public static void Begin(bool superEvolve)
    {
        IsActive = true;
        IsSuperEvolveMode = superEvolve;
    }

    public static void End()
    {
        IsActive = false;
    }

    public static bool IsEvolvable(CardModel card)
    {
        if (!IsActive) return false;
        return IsSuperEvolveMode ? EvoCmd.CanSuperEvolve(card) : EvoCmd.CanEvolve(card);
    }
}
