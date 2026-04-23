using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Omen;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

// Evolve: Add 2 White Psalm, New Revelation to your hand.
public class LishennaMelodyManifestEvolved : LishennaMelodyManifest
{
    public LishennaMelodyManifestEvolved() : this(EvoTier.Evolved) { }
    protected LishennaMelodyManifestEvolved(EvoTier tier) : base(tier) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await WhitePsalmNewRevelation.CreateInHand(Owner, CombatState);
        await WhitePsalmNewRevelation.CreateInHand(Owner, CombatState);
    }
}
