using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class DefendPortalcraftEvolved : DefendPortalcraft
{
    public DefendPortalcraftEvolved() : this(EvoTier.Evolved) { }
    protected DefendPortalcraftEvolved(EvoTier tier) : base(tier) { }
}
