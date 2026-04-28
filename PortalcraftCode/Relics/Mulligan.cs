using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace sts2_char_portalcraft.PortalcraftCode.Relics;

public sealed class Mulligan : PortalcraftRelic
{
    private const int MaxReturned = 4;

    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        if ((player.Creature.CombatState?.RoundNumber ?? 0) > 1) return;

        var hand = PileType.Hand.GetPile(player);
        if (hand.Cards.Count == 0) return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, MaxReturned);
        var selected = (await CardSelectCmd.FromHand(choiceContext, player, prefs, null, this)).ToArray();
        if (selected.Length == 0) return;

        Flash();
        await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Random);
        await CardPileCmd.Shuffle(choiceContext, player);
        await CardPileCmd.Draw(choiceContext, selected.Length, player);
    }
}
