using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Sincerity of the Dewdrop — 1 cost Uncommon Skill.
/// Transform a card in your hand into Imari's Little Buddies.
/// Upgrade: Transform up to 2 cards into upgraded Imari's Little Buddies.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class SincerityOfTheDewdrop : sts2_char_portalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<ImarisLittleBuddies>(),
    };

    public SincerityOfTheDewdrop() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool Filter(CardModel c) => c != this;
        int maxSelect = IsUpgraded ? 2 : 1;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "SINCERITY_PROMPT"),
            minCount: 1,
            maxCount: maxSelect);

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;

        foreach (var card in selected)
        {
            var result = await CardCmd.TransformTo<ImarisLittleBuddies>(card);
            if (IsUpgraded && result.HasValue)
            {
                CardCmd.Upgrade(result.Value.cardAdded);
            }
        }
    }
}
