using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class EustaceHowlOfThunderEvolved : EustaceHowlOfThunder
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(20m, ValueProp.Move),
    };

    public EustaceHowlOfThunderEvolved() : this(EvoTier.Evolved) { }
    protected EustaceHowlOfThunderEvolved(EvoTier tier) : base(tier) { }
}
