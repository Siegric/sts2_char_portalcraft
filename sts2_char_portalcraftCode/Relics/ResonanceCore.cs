using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Relics;

/// <summary>
/// Resonance Core — Starter relic.
/// At the start of combat, add a Gear of Ambition and a Gear of Remembrance to your hand.
/// </summary>
public sealed class ResonanceCore : sts2_char_portalcraftRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        if (player.Creature.CombatState.RoundNumber > 1) return;

        Flash();

        var combatState = player.Creature.CombatState;
        var gear1 = combatState.CreateCard<GearOfAmbition>(player);
        await CardPileCmd.AddGeneratedCardToCombat(gear1, PileType.Hand, addedByPlayer: true);

        var gear2 = combatState.CreateCard<GearOfRemembrance>(player);
        await CardPileCmd.AddGeneratedCardToCombat(gear2, PileType.Hand, addedByPlayer: true);
    }
}
