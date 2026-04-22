using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class TsubasaBlazingGearcyclist : PortalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 3m),
    };

    public TsubasaBlazingGearcyclist() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int count = (int)DynamicVars["MagicNumber"].BaseValue;
        var handCards = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != this)
            .ToList();

        Owner.RunState.Rng.Shuffle.Shuffle(handCards);
        foreach (var card in handCards.Take(count))
        {
            card.EnergyCost.AddThisTurn(-1);
        }

        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(1m);
    }
}
