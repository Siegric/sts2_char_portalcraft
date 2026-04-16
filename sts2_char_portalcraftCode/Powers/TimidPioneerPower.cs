using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;


public class TimidPioneerPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<TimidPioneer>();

    protected override bool IsPositive => false;
}
