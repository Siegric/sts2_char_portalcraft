using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class LudicrousOrdnanceEvolved : LudicrousOrdnance
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(14m, ValueProp.Move),
    };

    public LudicrousOrdnanceEvolved() : this(EvoTier.Evolved) { }
    protected LudicrousOrdnanceEvolved(EvoTier tier) : base(tier) { }
}
