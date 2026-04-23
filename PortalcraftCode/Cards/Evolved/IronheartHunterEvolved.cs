using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

// Evolve: deal 3 additional damage.
// Implemented as a base-damage override (9 vs 6), so the display reflects the
// buff directly and the runtime evolve/super-evolve bonus from KeywordDispatcher
// stacks on top as expected (Evolved = 9+2 = 11, SuperEvolved = 9+3 = 12).
public class IronheartHunterEvolved : IronheartHunter
{
    public IronheartHunterEvolved() : this(EvoTier.Evolved) { }
    protected IronheartHunterEvolved(EvoTier tier) : base(tier) { }

    protected override decimal BaseDamage => 9m;
}
