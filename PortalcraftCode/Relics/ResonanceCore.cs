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
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<KeywordDispatcherPower>(Owner.Creature, 1, Owner.Creature, null);
        await PowerCmd.Apply<EvoPointsPower>(Owner.Creature, EvoRuntime.MaxEvoPoints, Owner.Creature, null);
        await PowerCmd.Apply<SuperEvoPointsPower>(Owner.Creature, EvoRuntime.MaxSuperEvoPoints, Owner.Creature, null);
        await PowerCmd.Apply<SkyboundArtGaugePower>(Owner.Creature, 1, Owner.Creature, null);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        if (player.Creature.CombatState.RoundNumber > 1) return;

        Flash();
    }
}
