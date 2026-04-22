using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

public interface ICrystallizeCard
{
    int CrystallizeCost { get; }
    
    Type AmuletFormType { get; }
    
    Task OnAmuletSpawned(PlayerChoiceContext choiceContext, CardModel amulet) => Task.CompletedTask;
}
