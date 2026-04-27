using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class PuppetCatEvolved : PuppetCat
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(6m, ValueProp.Move),
        new IntVar("PuppetBonus", 3m),
    };

    public PuppetCatEvolved() : this(EvoTier.Evolved) { }
    protected PuppetCatEvolved(EvoTier tier) : base(tier) { }
}
