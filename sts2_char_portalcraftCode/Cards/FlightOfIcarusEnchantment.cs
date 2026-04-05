using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

public sealed class FlightOfIcarusEnchantment : EnchantmentModel
{
    public override bool HasExtraCardText => true;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (Status == EnchantmentStatus.Normal)
        {
            await CardPileCmd.Draw(choiceContext, 1, Card.Owner);
            Status = EnchantmentStatus.Disabled;
        }
    }
}
