using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class EnginebladeMavenEvolved : EnginebladeMaven
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(12m, ValueProp.Move),
    };

    public EnginebladeMavenEvolved() : this(EvoTier.Evolved) { }
    protected EnginebladeMavenEvolved(EvoTier tier) : base(tier) { }
}
