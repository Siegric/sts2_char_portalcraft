using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;

public static class PuppetHelper
{
    public static int CountPuppetsInHand(Player owner)
    {
        return PileType.Hand.GetPile(owner).Cards
            .Count(c => c.Tags.Contains(PuppetTag.Puppet));
    }
    
    public static int CountPuppetsPlayedThisTurn(Player owner, ICombatState combatState)
    {
        return CombatManager.Instance.History.CardPlaysStarted
            .Count(e => e.HappenedThisTurn(combatState)
                        && e.CardPlay.Card.Owner == owner
                        && e.CardPlay.Card.Tags.Contains(PuppetTag.Puppet));
    }
    
    public static bool IsPuppet(CardModel card)
    {
        return card.Tags.Contains(PuppetTag.Puppet);
    }
}
