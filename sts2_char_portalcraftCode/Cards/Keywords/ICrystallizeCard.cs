using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;

public interface ICrystallizeCard
{
    int CrystallizeCost { get; }

    // The CardModel subclass to spawn in hand when this card is played via Crystallize.
    // Must be an Amulet-type card (Retain + Unplayable, Type = AmuletType.Amulet).
    Type AmuletFormType { get; }

    // Called by the Crystallize play patch after the amulet form has been added to hand.
    // Use this to apply any paired ongoing power or other setup bound to the amulet.
    Task OnAmuletSpawned(PlayerChoiceContext choiceContext, CardModel amulet) => Task.CompletedTask;
}
