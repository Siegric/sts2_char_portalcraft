using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class IsaacCongenialEngineerEvolved : IsaacCongenialEngineer
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new BlockVar(6m, ValueProp.Move),
    };

    public IsaacCongenialEngineerEvolved() : this(EvoTier.Evolved) { }
    protected IsaacCongenialEngineerEvolved(EvoTier tier) : base(tier) { }
}
