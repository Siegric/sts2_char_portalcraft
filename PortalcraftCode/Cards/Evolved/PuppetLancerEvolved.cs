using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class PuppetLancerEvolved : PuppetLancer
{
    public PuppetLancerEvolved() : this(EvoTier.Evolved) { }
    protected PuppetLancerEvolved(EvoTier tier) : base(tier) { }
}
