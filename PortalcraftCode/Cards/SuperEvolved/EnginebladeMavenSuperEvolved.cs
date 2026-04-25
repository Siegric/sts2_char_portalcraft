using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;

public class EnginebladeMavenSuperEvolved : EnginebladeMavenEvolved
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(13m, ValueProp.Move),
    };

    public EnginebladeMavenSuperEvolved() : base(EvoTier.SuperEvolved) { }
}
