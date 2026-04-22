using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class AchimLordOfDespair : PortalcraftCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public AchimLordOfDespair() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var exhaustPrefs = new CardSelectorPrefs(
            new LocString("card_selection", "ACHIM_EXHAUST_PROMPT"),
            minCount: 1,
            maxCount: 1);

        var toExhaust = (await CardSelectCmd.FromHand(choiceContext, Owner, exhaustPrefs, c => c != this, this)).ToList();
        if (toExhaust.Count > 0)
        {
            await CardCmd.Exhaust(choiceContext, toExhaust[0]);
        }
        
        int retrieveCount = 1;
        var discardCards = PileType.Discard.GetPile(Owner).Cards.ToList();
        if (discardCards.Count == 0) return;

        var retrievePrefs = new CardSelectorPrefs(
            new LocString("card_selection", "ACHIM_RETRIEVE_PROMPT"),
            minCount: 1,
            maxCount: retrieveCount);

        var toRetrieve = (await CardSelectCmd.FromSimpleGrid(choiceContext, discardCards, Owner, retrievePrefs)).ToList();
        foreach (var card in toRetrieve)
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
