using MegaCrit.Sts2.Core.Entities.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

/// <summary>
/// Beelzebub power: Adds a flat bonus to all Talisman outputs.
/// Each stack adds +2 to heal, block, and damage.
/// Stack 1 = +2, Stack 2 = +4, etc.
/// This is a marker power checked by TalismanPower when calculating Psalm values.
/// </summary>
public sealed class BeelzebubSupremeKingPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// Returns the flat bonus per value: 2 * Amount (stacks).
    /// E.g., 1 stack = +2, 2 stacks = +4.
    /// </summary>
    public int Bonus => 2 * Amount;
}
