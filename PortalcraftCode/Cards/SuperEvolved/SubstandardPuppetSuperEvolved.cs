using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;

public class SubstandardPuppetSuperEvolved : SubstandardPuppetEvolved
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(17m, ValueProp.Move),
    };

    public SubstandardPuppetSuperEvolved() : base(EvoTier.SuperEvolved) { }
}
