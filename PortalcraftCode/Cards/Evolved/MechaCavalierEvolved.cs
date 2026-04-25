using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class MechaCavalierEvolved : MechaCavalier
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new BlockVar(12m, ValueProp.Move),
    };

    public MechaCavalierEvolved() : this(EvoTier.Evolved) { }
    protected MechaCavalierEvolved(EvoTier tier) : base(tier) { }
}
