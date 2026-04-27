using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;

public class KittyCannoneerSuperEvolved : KittyCannoneerEvolved
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new DamageVar(13m, ValueProp.Move),
        new IntVar("MagicNumber", 1m),
        };

    public KittyCannoneerSuperEvolved() : base(EvoTier.SuperEvolved) { }
}
