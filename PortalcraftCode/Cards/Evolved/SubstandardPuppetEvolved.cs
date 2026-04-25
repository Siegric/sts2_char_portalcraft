using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class SubstandardPuppetEvolved : SubstandardPuppet
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {       
        new DamageVar(16m, ValueProp.Move),
    };

    public SubstandardPuppetEvolved() : this(EvoTier.Evolved) { }
    protected SubstandardPuppetEvolved(EvoTier tier) : base(tier) { }
}
