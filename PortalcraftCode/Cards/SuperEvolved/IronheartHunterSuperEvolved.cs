using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;

// Inherits BaseDamage override from IronheartHunterEvolved (9m) — super-evolve
// triggers the Evolve: effect per the "super-evolve counts as evolve" rule.
public class IronheartHunterSuperEvolved : IronheartHunterEvolved
{
    public IronheartHunterSuperEvolved() : base(EvoTier.SuperEvolved) { }
}
