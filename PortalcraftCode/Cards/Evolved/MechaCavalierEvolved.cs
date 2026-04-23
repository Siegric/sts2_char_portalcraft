using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class MechaCavalierEvolved : MechaCavalier
{
    public MechaCavalierEvolved() : this(EvoTier.Evolved) { }
    protected MechaCavalierEvolved(EvoTier tier) : base(tier) { }
}
