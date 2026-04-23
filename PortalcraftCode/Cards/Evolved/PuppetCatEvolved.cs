using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class PuppetCatEvolved : PuppetCat
{
    public PuppetCatEvolved() : this(EvoTier.Evolved) { }
    protected PuppetCatEvolved(EvoTier tier) : base(tier) { }
}
