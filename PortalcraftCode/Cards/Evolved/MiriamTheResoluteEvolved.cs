using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

// Evolve: Replicate this card's effects — runs the base effect twice per play.
public class MiriamTheResoluteEvolved : MiriamTheResolute
{
    public MiriamTheResoluteEvolved() : this(EvoTier.Evolved) { }
    protected MiriamTheResoluteEvolved(EvoTier tier) : base(tier) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await RunEffect();
        await RunEffect();
    }
}
