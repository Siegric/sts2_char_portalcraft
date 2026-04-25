using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class LovestruckPuppeteerEvolved : LovestruckPuppeteer
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {        
        new BlockVar(10m, ValueProp.Move),
    };

    public LovestruckPuppeteerEvolved() : this(EvoTier.Evolved) { }
    protected LovestruckPuppeteerEvolved(EvoTier tier) : base(tier) { }
}
