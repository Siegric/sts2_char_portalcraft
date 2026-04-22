using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class IlsaBrutalDrillSergeant : PortalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<IlsaBarrageChoice>(),
        HoverTipFactory.FromCard<IlsaSweepChoice>(),
    };

    public IlsaBrutalDrillSergeant() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var barrage = CombatState.CreateCard<IlsaBarrageChoice>(Owner);
        var sweep = CombatState.CreateCard<IlsaSweepChoice>(Owner);

        if (IsUpgraded)
        {
            barrage.DynamicVars.Damage.UpgradeValueBy(4m);
            sweep.DynamicVars.Damage.UpgradeValueBy(4m);
        }

        var cards = new List<CardModel> { barrage, sweep };
        var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner);

        if (chosen != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, addedByPlayer: true);
            await CardCmd.AutoPlay(choiceContext, chosen, null);
        }
    }
}
