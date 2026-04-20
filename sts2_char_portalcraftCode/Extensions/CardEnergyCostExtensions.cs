using System.Collections;
using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Extensions;

public static class CardEnergyCostExtensions
{
    private static readonly FieldInfo LocalModifiersField =
        typeof(CardEnergyCost).GetField("_localModifiers",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

    public static void ClearLocalCostModifiers(this CardEnergyCost cost)
    {
        ((IList)LocalModifiersField.GetValue(cost)!).Clear();
    }
}
