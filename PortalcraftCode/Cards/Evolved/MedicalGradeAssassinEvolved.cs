using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class MedicalGradeAssassinEvolved : MedicalGradeAssassin
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(8m, ValueProp.Move),
    };

    public MedicalGradeAssassinEvolved() : this(EvoTier.Evolved) { }
    protected MedicalGradeAssassinEvolved(EvoTier tier) : base(tier) { }
}
