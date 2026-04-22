using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

public sealed class FlightOfIcarusEnchantment : EnchantmentModel, ILastWordsEnchantment
{
    public override bool HasExtraCardText => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(LastWordsKeyword.LastWords),
    };

    public async Task OnLastWords(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (Status != EnchantmentStatus.Normal) return;
        await CardPileCmd.Draw(choiceContext, 1, card.Owner);
        Status = EnchantmentStatus.Disabled;
    }
}
