using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;

public class MechanizedBeastSuperEvolved : MechanizedBeastEvolved
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new BlockVar(21m, ValueProp.Move),
        new IntVar("MagicNumber", 6m),
    };

    public MechanizedBeastSuperEvolved() : base(EvoTier.SuperEvolved) { }
}
