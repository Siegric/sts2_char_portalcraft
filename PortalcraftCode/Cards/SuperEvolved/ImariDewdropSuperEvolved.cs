using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.SuperEvolved;

// Super-Evolve: Draw 2 additional random Skills from the draw pile.
public class ImariDewdropSuperEvolved : ImariDewdropEvolved
{
    public ImariDewdropSuperEvolved() : base(EvoTier.SuperEvolved) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await DrawRandomSkills(Owner, 2);
    }
}
