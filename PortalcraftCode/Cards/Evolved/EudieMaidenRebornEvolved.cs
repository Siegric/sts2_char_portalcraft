using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

// Evolve: Gain the Crest (apply EudieMaidenRebornPower) after drawing.
public class EudieMaidenRebornEvolved : EudieMaidenReborn
{
    public EudieMaidenRebornEvolved() : this(EvoTier.Evolved) { }
    protected EudieMaidenRebornEvolved(EvoTier tier) : base(tier) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await PowerCmd.Apply<EudieMaidenRebornPower>(Owner.Creature, 1, Owner.Creature, this);
    }
}
