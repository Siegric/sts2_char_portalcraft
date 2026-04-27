using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class YogZenthaEldAxeEvolved : YogZenthaEldAxe
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(10m, ValueProp.Move),
    };

    public YogZenthaEldAxeEvolved() : this(EvoTier.Evolved) { }
    protected YogZenthaEldAxeEvolved(EvoTier tier) : base(tier) { }
}
