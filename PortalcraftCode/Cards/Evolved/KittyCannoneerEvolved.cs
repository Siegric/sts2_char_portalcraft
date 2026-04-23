using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class KittyCannoneerEvolved : KittyCannoneer
{
    public KittyCannoneerEvolved() : this(EvoTier.Evolved) { }
    protected KittyCannoneerEvolved(EvoTier tier) : base(tier) { }
}
