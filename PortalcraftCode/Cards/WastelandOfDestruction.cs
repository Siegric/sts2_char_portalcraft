using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Omen;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class WastelandOfDestruction : PortalcraftCard, ILastWordsCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Cards", 2m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        LastWordsKeyword.LastWords,
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<WastelandOfDestructionToken>(),
    };

    public WastelandOfDestruction() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool AnyCard(CardModel c) => c != this;
        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(AnyCard).ToList();

        if (handCards.Count > 0)
        {
            var exhaustPrefs = new CardSelectorPrefs(
                new LocString("card_selection", "WASTELAND_PROMPT"),
                minCount: 0,
                maxCount: 1
            );

            var toExhaust = (await CardSelectCmd.FromHand(choiceContext, Owner, exhaustPrefs, AnyCard, this)).ToList();
            if (toExhaust.Count > 0)
            {
                foreach (var card in toExhaust)
                {
                    await CardCmd.Exhaust(choiceContext, card);
                }

                int drawCount = (int)DynamicVars["Cards"].BaseValue;
                await CardPileCmd.Draw(choiceContext, drawCount, Owner);
            }
        }

        await WastelandOfDestructionToken.CreateInHand(Owner, CombatState);
    }

    public async Task OnLastWords(PlayerChoiceContext choiceContext)
    {
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
