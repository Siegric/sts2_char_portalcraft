using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

/// <summary>
/// Puppet Theater power: At the start of each turn, add a Puppet to your hand.
/// </summary>
public sealed class PuppetTheaterPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;

        Flash();
        await Puppet.CreateInHand(player, Amount, CombatState);
    }
}
