using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Powers;

// Ongoing effect while one or more RetrafiaDivineMotherAmulet cards exist in the player's piles:
//   - All Artifact cards owned by the player cost 0.
//   - When the player plays an Artifact, every Retrafia amulet's Countdown ticks by 1.
// Removes itself when the last amulet leaves play.
public sealed class RetrafiaAmuletPower : sts2_char_portalcraftPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card.Owner?.Creature != Owner || card is not ArtifactCard)
        {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = 0m;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner) return;
        if (cardPlay.Card is not ArtifactCard) return;

        var amulets = CollectAmulets();
        if (amulets.Count == 0) return;

        Flash();
        foreach (var amulet in amulets)
        {
            await CountdownHelper.Tick(choiceContext, amulet);
        }
    }

    public override Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner?.Creature != Owner) return Task.CompletedTask;
        if (card is not RetrafiaDivineMotherAmulet) return Task.CompletedTask;

        if (CollectAmulets().Count == 0)
        {
            Owner.RemovePowerInternal(this);
        }
        return Task.CompletedTask;
    }

    private List<CardModel> CollectAmulets()
    {
        var player = CombatState.Players.FirstOrDefault(p => p.Creature == Owner);
        if (player == null) return new List<CardModel>();

        return new[] { PileType.Hand, PileType.Draw, PileType.Discard, PileType.Play }
            .SelectMany(p => p.GetPile(player).Cards)
            .OfType<RetrafiaDivineMotherAmulet>()
            .Cast<CardModel>()
            .ToList();
    }
}
