using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

// Gauge lives on the player's creature as SkyboundArtGaugePower.Amount.
// All mutations go through networked PowerCmd.* so both clients stay in sync.
// Threshold crossings apply invisible signal powers (SkyboundArtPower /
// SuperSkyboundArtPower) whose presence gates per-card firing.
public static class SkyboundArtRuntime
{
    public const int SkyboundArtThreshold = 10;
    public const int SuperSkyboundArtThreshold = 15;

    public static int CurrentGauge(Player player) =>
        (int)(player.Creature.GetPower<SkyboundArtGaugePower>()?.Amount ?? 0m);

    public static bool IsActive(Player player) =>
        player.Creature.HasPower<SkyboundArtPower>();

    public static bool IsSuperActive(Player player) =>
        player.Creature.HasPower<SuperSkyboundArtPower>();

    public static async Task AddGaugeBonus(Player player, int amount, PlayerChoiceContext ctx)
    {
        var gauge = player.Creature.GetPower<SkyboundArtGaugePower>();
        if (gauge == null) return;
        await PowerCmd.ModifyAmount(gauge, amount, null, null, silent: true);
        await CheckThresholds(player, ctx);
    }

    private static async Task CheckThresholds(Player player, PlayerChoiceContext ctx)
    {
        int gauge = CurrentGauge(player);
        var creature = player.Creature;

        if (gauge >= SkyboundArtThreshold && !creature.HasPower<SkyboundArtPower>())
        {
            await PowerCmd.Apply<SkyboundArtPower>(creature, 1m, creature, null, silent: true);
            await FireHandSkyboundArt(player, ctx, isSuper: false);
        }
        if (gauge >= SuperSkyboundArtThreshold && !creature.HasPower<SuperSkyboundArtPower>())
        {
            await PowerCmd.Apply<SuperSkyboundArtPower>(creature, 1m, creature, null, silent: true);
            await FireHandSkyboundArt(player, ctx, isSuper: true);
        }
    }

    private static async Task FireHandSkyboundArt(Player player, PlayerChoiceContext ctx, bool isSuper)
    {
        CardKeyword targetKeyword = isSuper
            ? SuperSkyboundArtKeyword.SuperSkyboundArt
            : SkyboundArtKeyword.SkyboundArt;

        var cards = PileType.Hand.GetPile(player).Cards
            .Where(c => c is ISkyboundArtCard && c.Keywords.Contains(targetKeyword))
            .ToList();
        foreach (var card in cards)
        {
            await FireAsAutoPlay(card, ctx);
        }
    }

    // Auto-plays the card through the game's normal play pipeline so the visuals
    // and consume-on-play behaviour match a real play. OnPlay on the card checks
    // for SkyboundArtAutoPlayingPower and short-circuits to the art effect rather
    // than running the regular play effect. Both the apply and remove of the flag
    // go through networked PowerCmd so both clients take the same branch.
    public static async Task FireAsAutoPlay(CardModel card, PlayerChoiceContext ctx)
    {
        if (card.Owner == null) return;
        var creature = card.Owner.Creature;

        await PowerCmd.Apply<SkyboundArtAutoPlayingPower>(creature, 1m, creature, null, silent: true);
        try
        {
            await CardCmd.AutoPlay(ctx, card, null);
        }
        finally
        {
            var flag = creature.GetPower<SkyboundArtAutoPlayingPower>();
            if (flag != null) await PowerCmd.Remove(flag);
        }
    }
}
