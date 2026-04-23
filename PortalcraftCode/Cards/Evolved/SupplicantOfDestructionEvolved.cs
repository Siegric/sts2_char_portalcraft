using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class SupplicantOfDestructionEvolved : SupplicantOfDestruction
{
    public SupplicantOfDestructionEvolved() : this(EvoTier.Evolved) { }
    protected SupplicantOfDestructionEvolved(EvoTier tier) : base(tier) { }
}
