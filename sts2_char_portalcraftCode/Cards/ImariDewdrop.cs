using System.Collections.Generic;
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
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Imari, Dewdrop — 2 cost Rare Skill.
/// Select a card in your hand and discard it. Search your draw pile for a Skill and add it to your hand.
/// This turn, whenever you play a Skill, add Imari's Little Buddies to your hand.
/// Upgrade: Add upgraded Imari's Little Buddies instead.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class ImariDewdrop : sts2_char_portalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<ImarisLittleBuddies>(),
        HoverTipFactory.FromPower<ImariDewdropPower>(),
    };

    public ImariDewdrop() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Step 1: Discard a card from hand
        bool AnyCard(CardModel c) => c != this;
        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(AnyCard).ToList();
        if (handCards.Count > 0)
        {
            var discardPrefs = new CardSelectorPrefs(
                new LocString("card_selection", "IMARI_DISCARD_PROMPT"),
                minCount: 1,
                maxCount: 1);

            var toDiscard = (await CardSelectCmd.FromHand(choiceContext, Owner, discardPrefs, AnyCard, this)).ToList();
            if (toDiscard.Count == 0) return;
            await CardCmd.Discard(choiceContext, toDiscard[0]);
        }

        // Step 2: Tutor a Skill from draw pile
        var skills = PileType.Draw.GetPile(Owner).Cards.Where(c => c.Type == CardType.Skill).ToList();
        if (skills.Count > 0)
        {
            var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, skills, Owner);
            if (chosen != null)
            {
                await CardPileCmd.Add(chosen, PileType.Hand);
            }
        }

        // Step 3: Apply temporary power — Skills generate Imari tokens this turn
        int amount = IsUpgraded ? 2 : 1;
        await PowerCmd.Apply<ImariDewdropPower>(Owner.Creature, amount, Owner.Creature, this);
    }
}
