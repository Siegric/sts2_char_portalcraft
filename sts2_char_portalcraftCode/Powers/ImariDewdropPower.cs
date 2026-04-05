using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

/// <summary>
/// Imari, Dewdrop power: This turn, whenever you play a Skill, add Imari's Little Buddies to hand.
/// Amount > 1 means add upgraded versions.
/// Expires at end of turn.
/// </summary>
public sealed class ImariDewdropPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner) return;
        if (cardPlay.Card.Type != CardType.Skill) return;

        Flash();
        var token = CombatState.CreateCard<Cards.ImarisLittleBuddies>(cardPlay.Card.Owner);
        if (Amount > 1)
        {
            CardCmd.Upgrade(token);
        }
        await CardPileCmd.AddGeneratedCardToCombat(token, PileType.Hand, addedByPlayer: true);
    }

    public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player)
        {
            Owner.RemovePowerInternal(this);
        }
        return Task.CompletedTask;
    }
}
