using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class RukinaResistanceLeaderEvolved : RukinaResistanceLeader
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new BlockVar(10m, ValueProp.Move),
    };

    public RukinaResistanceLeaderEvolved() : this(EvoTier.Evolved) { }
    protected RukinaResistanceLeaderEvolved(EvoTier tier) : base(tier) { }
}
