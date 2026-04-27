using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class LunarBunnyEvolved : LunarBunny
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new BlockVar(11m, ValueProp.Move),
    };

    public LunarBunnyEvolved() : this(EvoTier.Evolved) { }
    protected LunarBunnyEvolved(EvoTier tier) : base(tier) { }
}
