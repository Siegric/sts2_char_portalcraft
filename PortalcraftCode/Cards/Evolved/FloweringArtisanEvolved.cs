using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class FloweringArtisanEvolved : FloweringArtisan
{
    public FloweringArtisanEvolved() : this(EvoTier.Evolved) { }
    protected FloweringArtisanEvolved(EvoTier tier) : base(tier) { }
}
