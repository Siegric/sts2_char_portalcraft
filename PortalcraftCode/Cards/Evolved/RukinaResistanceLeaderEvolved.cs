using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

// Evolve: Add a Striker Artifact with Ethereal and 0-cost-this-turn.
public class RukinaResistanceLeaderEvolved : RukinaResistanceLeader
{
    public RukinaResistanceLeaderEvolved() : this(EvoTier.Evolved) { }
    protected RukinaResistanceLeaderEvolved(EvoTier tier) : base(tier) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        var striker = CombatState.CreateCard<StrikerArtifact>(Owner);
        striker.AddKeyword(CardKeyword.Ethereal);
        striker.EnergyCost.SetThisTurnOrUntilPlayed(0, reduceOnly: true);
        await CardPileCmd.AddGeneratedCardToCombat(striker, PileType.Hand, addedByPlayer: true);
    }
}
