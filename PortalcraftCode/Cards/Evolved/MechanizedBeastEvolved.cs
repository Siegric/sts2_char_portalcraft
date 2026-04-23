using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class MechanizedBeastEvolved : MechanizedBeast
{
    public MechanizedBeastEvolved() : this(EvoTier.Evolved) { }
    protected MechanizedBeastEvolved(EvoTier tier) : base(tier) { }
}
