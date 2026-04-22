using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public sealed class LiamCrazedCreatorPower : PortalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _puppetPlayCount;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner?.Creature != Owner) return;
        if (!PuppetHelper.IsPuppet(card)) return;

        Flash();
        
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        
        _puppetPlayCount++;
        if (_puppetPlayCount >= 4)
        {
            _puppetPlayCount = 0;
            await PlayerCmd.GainEnergy(1, card.Owner);
        }
    }
}
