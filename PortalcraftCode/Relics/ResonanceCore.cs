using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Relics;

public sealed class ResonanceCore : PortalcraftRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task BeforeCombatStart()
    {
        await PowerCmd.Apply<KeywordDispatcherPower>(Owner.Creature, 1, Owner.Creature, null);

        if (Owner.PlayerCombatState != null)
        {
            EvoRuntime.InitForCombat(Owner.PlayerCombatState);
        }
    }

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
