using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class StrikePortalcraftEvolved : StrikePortalcraft
{
    public StrikePortalcraftEvolved() : this(EvoTier.Evolved) { }
    protected StrikePortalcraftEvolved(EvoTier tier) : base(tier) { }
}
