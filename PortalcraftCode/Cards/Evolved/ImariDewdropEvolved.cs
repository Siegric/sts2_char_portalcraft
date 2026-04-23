using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Evolved;

// Evolve: Whenever you play a Skill this turn, add an Imari's Little Buddies
// to your hand. Implemented by applying ImariDewdropPower on every play of the
// evolved card (power self-removes at turn end).
public class ImariDewdropEvolved : ImariDewdrop
{
    public ImariDewdropEvolved() : this(EvoTier.Evolved) { }
    protected ImariDewdropEvolved(EvoTier tier) : base(tier) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        await PowerCmd.Apply<ImariDewdropPower>(Owner.Creature, 1, Owner.Creature, this);
        if (IsUpgraded)
        {
            Owner.Creature.GetPower<ImariDewdropPower>()!.Upgraded = true;
        }
    }
}
