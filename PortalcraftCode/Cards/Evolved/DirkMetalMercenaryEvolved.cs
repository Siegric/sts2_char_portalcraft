using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class DirkMetalMercenaryEvolved : DirkMetalMercenary
{
    public DirkMetalMercenaryEvolved() : this(EvoTier.Evolved) { }
    protected DirkMetalMercenaryEvolved(EvoTier tier) : base(tier) { }
}
