using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class LudicrousOrdnanceEvolved : LudicrousOrdnance
{
    public LudicrousOrdnanceEvolved() : this(EvoTier.Evolved) { }
    protected LudicrousOrdnanceEvolved(EvoTier tier) : base(tier) { }
}
