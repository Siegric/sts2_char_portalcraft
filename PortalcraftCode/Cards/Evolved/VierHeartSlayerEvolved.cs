using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class VierHeartSlayerEvolved : VierHeartSlayer
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(6m, ValueProp.Move),
    };

    public VierHeartSlayerEvolved() : this(EvoTier.Evolved) { }
    protected VierHeartSlayerEvolved(EvoTier tier) : base(tier) { }
}
