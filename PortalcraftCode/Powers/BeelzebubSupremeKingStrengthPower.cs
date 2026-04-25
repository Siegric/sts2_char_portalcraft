using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using sts2_char_portalcraft.PortalcraftCode.Cards;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public class BeelzebubSupremeKingStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<BeelzebubSupremeKing>();

    protected override bool IsPositive => false;
}
