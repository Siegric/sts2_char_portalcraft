using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Audio;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class ImariDewdropPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public bool Upgraded { get; set; }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner) return;
        if (cardPlay.Card.Type != CardType.Skill) return;

        Flash();
        CardPlayAudioManager.PlayForEffect("ImariDewdrop");
        for (int i = 0; i < Amount; i++)
        {
            var token = CombatState.CreateCard<Cards.ImarisLittleBuddies>(cardPlay.Card.Owner);
            if (Upgraded)
            {
                CardCmd.Upgrade(token);
            }
            await CardPileCmd.AddGeneratedCardToCombat(token, PileType.Hand, addedByPlayer: true);
        }
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
