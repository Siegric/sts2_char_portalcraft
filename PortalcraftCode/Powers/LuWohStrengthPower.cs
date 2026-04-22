using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using sts2_char_portalcraft.PortalcraftCode.Cards;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public class LuWohStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<LuWohLightPersonified>();

    protected override bool IsPositive => false;
}
