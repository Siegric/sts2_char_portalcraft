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
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Omen;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Wasteland of Destruction — 1 cost Skill, Exhaust, Common.
/// Select a card in your hand and exhaust it. Draw 2 cards.
/// Add an unplayable copy of this card to your hand. If that copy is exhausted, gain 1 energy.
/// Upgrade: Draw 3 instead of 2.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class WastelandOfDestruction : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Cards", 2m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<WastelandOfDestructionToken>(),
    };

    public WastelandOfDestruction() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Select a card to exhaust
        bool AnyCard(CardModel c) => c != this;
        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(AnyCard).ToList();
        if (handCards.Count == 0) return;

        var exhaustPrefs = new CardSelectorPrefs(
            new LocString("card_selection", "WASTELAND_PROMPT"),
            minCount: 1,
            maxCount: 1
        );

        var toExhaust = (await CardSelectCmd.FromHand(choiceContext, Owner, exhaustPrefs, AnyCard, this)).ToList();
        if (toExhaust.Count == 0) return;

        bool didExhaust = false;
        foreach (var card in toExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
            didExhaust = true;
        }

        // Draw cards only if a card was actually exhausted
        if (didExhaust)
        {
            int drawCount = (int)DynamicVars["Cards"].BaseValue;
            await CardPileCmd.Draw(choiceContext, drawCount, Owner);
        }

        // Add unplayable Wasteland token to hand
        await WastelandOfDestructionToken.CreateInHand(Owner, CombatState);

        // Ensure TalismanPower exists to handle the wasteland token exhaust
        await TalismanHelper.EnsureTalismanPower(Owner, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
