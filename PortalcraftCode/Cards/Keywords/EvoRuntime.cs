using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public static class EvoRuntime
{
    public const int MaxEvoPoints = 2;
    public const int MaxSuperEvoPoints = 2;

    public const decimal EvolveDamageBonus = 2m;
    public const decimal EvolveBlockBonus = 2m;
    public const decimal SuperEvolveDamageBonus = 3m;
    public const decimal SuperEvolveBlockBonus = 3m;

    public enum Tier { Evolved, SuperEvolved }

    public static event Action<Player>? Changed;

    // --- Tier on card (SavedProperty) ---

    public static Tier? GetTier(CardModel card) =>
        card is IEvolvableCard ev
            ? ev.sts2_char_portalcraft_CurrentTier switch
            {
                EvoTier.Evolved => Tier.Evolved,
                EvoTier.SuperEvolved => Tier.SuperEvolved,
                _ => null,
            }
            : null;

    public static bool IsEvolved(CardModel card) =>
        (card as IEvolvableCard)?.sts2_char_portalcraft_CurrentTier == EvoTier.Evolved;

    public static bool IsSuperEvolved(CardModel card) =>
        (card as IEvolvableCard)?.sts2_char_portalcraft_CurrentTier == EvoTier.SuperEvolved;

    public static async Task MarkEvolved(CardModel card, PlayerChoiceContext ctx)
    {
        if (card is IEvolvableCard ev) ev.sts2_char_portalcraft_CurrentTier = EvoTier.Evolved;
        await BumpSkyboundArtGauge(card, ctx);
    }

    public static async Task MarkSuperEvolved(CardModel card, PlayerChoiceContext ctx)
    {
        if (card is IEvolvableCard ev) ev.sts2_char_portalcraft_CurrentTier = EvoTier.SuperEvolved;
        await BumpSkyboundArtGauge(card, ctx);
    }

    public static void ClearTier(CardModel card)
    {
        if (card is IEvolvableCard ev) ev.sts2_char_portalcraft_CurrentTier = EvoTier.Base;
    }

    private static async Task BumpSkyboundArtGauge(CardModel card, PlayerChoiceContext ctx)
    {
        if (card.Owner != null)
        {
            await SkyboundArtRuntime.AddGaugeBonus(card.Owner, 1, ctx);
        }
    }

    // --- Evo points + turn lockout on player creature via invisible powers ---
    // Using game-native powers means all state changes flow through the networked
    // PowerCmd.* commands and sync automatically. No client-local dict.

    public static int EvoPoints(Player player) =>
        (int)(player.Creature.GetPower<EvoPointsPower>()?.Amount ?? 0m);

    public static int SuperEvoPoints(Player player) =>
        (int)(player.Creature.GetPower<SuperEvoPointsPower>()?.Amount ?? 0m);

    public static bool UsedThisTurn(Player player) =>
        player.Creature.HasPower<EvoUsedThisTurnPower>();

    public static bool CanEvolve(Player player) =>
        EvoPoints(player) > 0 && !UsedThisTurn(player);

    public static bool CanSuperEvolve(Player player) =>
        SuperEvoPoints(player) > 0 && !UsedThisTurn(player);

    public static async Task<bool> TrySpendEvo(Player player, PlayerChoiceContext ctx)
    {
        if (!CanEvolve(player)) return false;
        var creature = player.Creature;
        var power = creature.GetPower<EvoPointsPower>();
        if (power == null) return false;
        await PowerCmd.ModifyAmount(ctx, power, -1m, null, null, silent: true);
        await PowerCmd.Apply<EvoUsedThisTurnPower>(ctx, creature, 1m, creature, null, silent: true);
        Changed?.Invoke(player);
        return true;
    }

    public static async Task<bool> TrySpendSuperEvo(Player player, PlayerChoiceContext ctx)
    {
        if (!CanSuperEvolve(player)) return false;
        var creature = player.Creature;
        var power = creature.GetPower<SuperEvoPointsPower>();
        if (power == null) return false;
        await PowerCmd.ModifyAmount(ctx, power, -1m, null, null, silent: true);
        await PowerCmd.Apply<EvoUsedThisTurnPower>(ctx, creature, 1m, creature, null, silent: true);
        Changed?.Invoke(player);
        return true;
    }

    public static async Task ResetTurnLockout(Player player)
    {
        var power = player.Creature.GetPower<EvoUsedThisTurnPower>();
        if (power == null) return;
        await PowerCmd.Remove(power);
        Changed?.Invoke(player);
    }
}
