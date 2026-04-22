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
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class ImariDewdrop : PortalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<ImarisLittleBuddies>(),
        HoverTipFactory.FromPower<ImariDewdropPower>(),
    };

    public ImariDewdrop() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool AnyCard(CardModel c) => c != this;
        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(AnyCard).ToList();
        if (handCards.Count > 0)
        {
            var discardPrefs = new CardSelectorPrefs(
                new LocString("card_selection", "IMARI_DISCARD_PROMPT"),
                minCount: 1,
                maxCount: 1);

            var toDiscard = (await CardSelectCmd.FromHand(choiceContext, Owner, discardPrefs, AnyCard, this)).ToList();
            if (toDiscard.Count > 0)
            {
                await CardCmd.Discard(choiceContext, toDiscard[0]);
            }
        }
        
        var skills = PileType.Draw.GetPile(Owner).Cards.Where(c => c.Type == CardType.Skill).ToList();
        if (skills.Count > 0)
        {
            var tutorPrefs = new CardSelectorPrefs(
                new LocString("card_selection", "IMARI_TUTOR_PROMPT"),
                minCount: 1,
                maxCount: 1);

            var chosen = (await CardSelectCmd.FromSimpleGrid(choiceContext, skills, Owner, tutorPrefs)).ToList();
            if (chosen.Count > 0)
            {
                await CardPileCmd.Add(chosen[0], PileType.Hand);
            }
        }
        
        await PowerCmd.Apply<ImariDewdropPower>(Owner.Creature, 1, Owner.Creature, this);
        if (IsUpgraded)
        {
            Owner.Creature.GetPower<ImariDewdropPower>()!.Upgraded = true;
        }
    }
}
