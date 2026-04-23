using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

public class MedicalGradeAssassinEvolved : MedicalGradeAssassin
{
    public MedicalGradeAssassinEvolved() : this(EvoTier.Evolved) { }
    protected MedicalGradeAssassinEvolved(EvoTier tier) : base(tier) { }
}
