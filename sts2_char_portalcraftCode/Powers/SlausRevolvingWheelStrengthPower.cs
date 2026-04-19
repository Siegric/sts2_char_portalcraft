using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

public sealed class SlausRevolvingWheelStrengthPower : TemporaryStrengthPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<SlausRevolvingWheelOfFortune>();
    protected override bool IsVisibleInternal => false;

    public string CustomPackedIconPath => "res://images/atlases/power_atlas.sprites/strength_power.tres";
    public string CustomBigIconPath => "res://images/powers/strength_power.png";
}
