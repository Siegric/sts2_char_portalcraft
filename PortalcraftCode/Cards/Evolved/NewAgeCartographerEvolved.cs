using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class NewAgeCartographerEvolved : NewAgeCartographer
{
    public NewAgeCartographerEvolved() : this(EvoTier.Evolved) { }
    protected NewAgeCartographerEvolved(EvoTier tier) : base(tier) { }
}
