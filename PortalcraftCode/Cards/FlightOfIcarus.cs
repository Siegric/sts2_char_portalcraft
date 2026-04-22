using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class FlightOfIcarus : PortalcraftCard
{
    public FlightOfIcarus() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "FLIGHT_OF_ICARUS_PROMPT"),
            minCount: 1,
            maxCount: 1
        );

        bool Filter(CardModel c) => c != this && c is ArtifactCard;

        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this);
        var card = selected.FirstOrDefault();

        if (card != null && card.Enchantment is not FlightOfIcarusEnchantment)
        {
            card.EnergyCost.SetThisTurnOrUntilPlayed(0, reduceOnly: true);
            CardCmd.Enchant<FlightOfIcarusEnchantment>(card, 1m);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
