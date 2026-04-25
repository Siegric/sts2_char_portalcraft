using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class ElectricWhipLassEvolved : ElectricWhipLass
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new BlockVar(8m, ValueProp.Move),
    };

    public ElectricWhipLassEvolved() : this(EvoTier.Evolved) { }
    protected ElectricWhipLassEvolved(EvoTier tier) : base(tier) { }
}
