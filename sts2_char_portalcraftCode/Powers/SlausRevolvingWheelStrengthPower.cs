using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

// Temporary Strength granted by Slaus, Revolving Wheel of Fortune (effect 2).
// Mirrors the vanilla pattern (e.g. CoordinatePower : TemporaryStrengthPower)
// — title, description, and turn-end removal are provided by the base class.
// ICustomPower implements ICustomModel (for BaseLib ID prefixing) and redirects
// the icons to the vanilla Strength art since this is semantically Strength.
public sealed class SlausRevolvingWheelStrengthPower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<SlausRevolvingWheelOfFortune>();

    // Hide this proxy from the status bar — the real StrengthPower it applies is already shown.
    protected override bool IsVisibleInternal => false;

    public string CustomPackedIconPath => "res://images/atlases/power_atlas.sprites/strength_power.tres";
    public string CustomBigIconPath => "res://images/powers/strength_power.png";
}
