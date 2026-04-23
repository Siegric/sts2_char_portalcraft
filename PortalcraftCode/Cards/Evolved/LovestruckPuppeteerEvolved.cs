using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

// Evolve: Add a Puppet to your hand. Fires on every OnPlay of the evolved card
// (not only at transform time), so re-drawing this evolved card retriggers the
// effect. SuperEvolved inherits this OnPlay via the class chain
// (SuperEvolved : Evolved) so it also re-triggers.
public class LovestruckPuppeteerEvolved : LovestruckPuppeteer
{
    public LovestruckPuppeteerEvolved() : this(EvoTier.Evolved) { }
    protected LovestruckPuppeteerEvolved(EvoTier tier) : base(tier) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await Puppet.CreateInHand(Owner, 1, CombatState);
    }
}
