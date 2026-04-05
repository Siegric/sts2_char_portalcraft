using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Congregant of Destruction — 1 cost Skill, Exhaust, Common.
/// Exhaust all cards in your hand. Draw X cards. X = cards exhausted this turn.
/// Upgrade: Cost -1.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class CongregantOfDestruction : sts2_char_portalcraftCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public CongregantOfDestruction() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Exhaust all cards in hand
        var handCards = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != this)
            .ToList();

        foreach (var card in handCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        // Draw X cards where X = total cards exhausted this turn
        int exhaustedThisTurn = PileType.Exhaust.GetPile(Owner).Cards.Count;
        if (exhaustedThisTurn > 0)
        {
            await CardPileCmd.Draw(choiceContext, exhaustedThisTurn, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
