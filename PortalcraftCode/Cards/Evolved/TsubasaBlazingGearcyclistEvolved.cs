using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class TsubasaBlazingGearcyclistEvolved : TsubasaBlazingGearcyclist
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(11m, ValueProp.Move),
    };

    public TsubasaBlazingGearcyclistEvolved() : this(EvoTier.Evolved) { }
    protected TsubasaBlazingGearcyclistEvolved(EvoTier tier) : base(tier) { }
}
