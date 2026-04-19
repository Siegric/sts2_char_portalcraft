using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

// Persistent, hidden power applied to the player at combat start by ResonanceCore.
// Dispatches the mod's custom card keywords:
//   - Countdown: decrements at each player turn start; exhausts at 0.
//   - LastWords: fires ILastWordsCard.OnLastWords when a card is exhausted.
public sealed class KeywordDispatcherPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    private int _artifactsExhausted;

    public int ArtifactsExhaustedCount => _artifactsExhausted;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;

        var scanned = new[] { PileType.Hand, PileType.Draw, PileType.Discard }
            .SelectMany(p => p.GetPile(player).Cards)
            .ToList();

        // Fire per-turn-start card effects first (before countdown tick, so the card
        // still exists for its final turn's effect).
        var turnStartCards = scanned.OfType<IOnTurnStartCard>().ToList();
        foreach (var card in turnStartCards)
        {
            await card.OnTurnStart(choiceContext);
        }

        var countdownCards = scanned.OfType<ICountdownCard>().Cast<CardModel>().ToList();
        foreach (var card in countdownCards)
        {
            Flash();
            await CountdownHelper.Tick(choiceContext, card);
        }
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner?.Creature != Owner) return;

        if (card is ArtifactCard)
        {
            _artifactsExhausted++;
        }

        if (card is ILastWordsCard lastWords)
        {
            Flash();
            await lastWords.OnLastWords(choiceContext);
        }

        if (card.Enchantment is ILastWordsEnchantment enchLastWords)
        {
            Flash();
            await enchLastWords.OnLastWords(choiceContext, card);
        }
    }
}
