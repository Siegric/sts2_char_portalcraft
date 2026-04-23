using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class SubstandardPuppetEvolved : SubstandardPuppet
{
    public SubstandardPuppetEvolved() : this(EvoTier.Evolved) { }
    protected SubstandardPuppetEvolved(EvoTier tier) : base(tier) { }
}
