using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class KittyCannoneerEvolved : KittyCannoneer
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(12m, ValueProp.Move),
        new IntVar("MagicNumber", 1m),
    };

    public KittyCannoneerEvolved() : this(EvoTier.Evolved) { }
    protected KittyCannoneerEvolved(EvoTier tier) : base(tier) { }
}
