using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class StrikePortalcraftEvolved : StrikePortalcraft
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {       
        new DamageVar(8m, ValueProp.Move),
    };

    public StrikePortalcraftEvolved() : this(EvoTier.Evolved) { }
    protected StrikePortalcraftEvolved(EvoTier tier) : base(tier) { }
}
