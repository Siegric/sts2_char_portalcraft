using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

public sealed class SlausRevolvingWheelDexterityPower : TemporaryDexterityPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<SlausRevolvingWheelOfFortune>();
    protected override bool IsVisibleInternal => false;

    public string CustomPackedIconPath => "res://images/atlases/power_atlas.sprites/dexterity_power.tres";
    public string CustomBigIconPath => "res://images/powers/dexterity_power.png";
}
