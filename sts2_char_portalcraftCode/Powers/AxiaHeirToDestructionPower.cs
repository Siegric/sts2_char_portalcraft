using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Omen;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

public sealed class AxiaHeirToDestructionPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;

        var player = CombatState.Players.FirstOrDefault(p => p.Creature == Owner);
        if (player == null) return;

        for (int round = 0; round < Amount; round++)
        {
            var psalms = PileType.Hand.GetPile(player).Cards
                .Where(c => AmuletHelper.IsWhitePsalm(c) || AmuletHelper.IsBlackPsalm(c))
                .ToList();

            if (psalms.Count == 0) break;

            Flash();
            foreach (var card in psalms)
            {
                await CardCmd.Exhaust(choiceContext, card);
            }
        }
    }
}
