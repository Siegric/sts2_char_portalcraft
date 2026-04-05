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
using MegaCrit.Sts2.Core.Random;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class ResurrectionTuner : sts2_char_portalcraftCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public ResurrectionTuner() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // First: exhaust 1 card from hand
        bool AnyCard(CardModel c) => c != this;

        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(AnyCard).ToList();
        if (handCards.Count == 0) return;

        var exhaustPrefs = new CardSelectorPrefs(
            new LocString("card_selection", "RESURRECTION_TUNER_EXHAUST_PROMPT"),
            minCount: 1,
            maxCount: 1
        );

        var toExhaust = (await CardSelectCmd.FromHand(choiceContext, Owner, exhaustPrefs, AnyCard, this)).ToList();
        if (toExhaust.Count == 0) return;

        foreach (var card in toExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        // Then: find up to 2 random puppets in exhaust pile and move them to hand
        var exhaustedPuppets = PileType.Exhaust.GetPile(Owner).Cards
            .Where(c => PuppetHelper.IsPuppet(c))
            .ToList();

        if (exhaustedPuppets.Count == 0) return;

        var rng = Owner.RunState.Rng.Shuffle;
        int count = System.Math.Min(2, exhaustedPuppets.Count);

        // Pick up to 2 random puppets
        var toRecover = new List<CardModel>();
        var pool = new List<CardModel>(exhaustedPuppets);
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            var picked = rng.NextItem(pool);
            toRecover.Add(picked);
            pool.Remove(picked);
        }

        foreach (var puppet in toRecover)
        {
            await CardPileCmd.Add(puppet, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
