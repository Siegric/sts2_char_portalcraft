using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class IsaacCongenialEngineerEvolved : IsaacCongenialEngineer
{
    public IsaacCongenialEngineerEvolved() : this(EvoTier.Evolved) { }
    protected IsaacCongenialEngineerEvolved(EvoTier tier) : base(tier) { }
}
