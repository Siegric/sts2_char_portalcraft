using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class MechanizedBeastEvolved : MechanizedBeast
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new BlockVar(20m, ValueProp.Move),
        new IntVar("MagicNumber", 6m),
    };

    public MechanizedBeastEvolved() : this(EvoTier.Evolved) { }
    protected MechanizedBeastEvolved(EvoTier tier) : base(tier) { }
}
