using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class EliseElectrifyingInventorEvolved : EliseElectrifyingInventor
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        new BlockVar(6m, ValueProp.Move),
        };

    public EliseElectrifyingInventorEvolved() : this(EvoTier.Evolved) { }
    protected EliseElectrifyingInventorEvolved(EvoTier tier) : base(tier) { }
}
