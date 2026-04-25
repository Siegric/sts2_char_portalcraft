using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class DefendPortalcraftEvolved : DefendPortalcraft
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new BlockVar(7m, ValueProp.Move),
    };

    public DefendPortalcraftEvolved() : this(EvoTier.Evolved) { }
    protected DefendPortalcraftEvolved(EvoTier tier) : base(tier) { }
}
