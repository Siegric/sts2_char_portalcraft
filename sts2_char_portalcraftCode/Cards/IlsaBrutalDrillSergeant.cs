using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Ilsa, Brutal Drill Sergeant — 3 cost Uncommon Attack.
/// Select a mode: 3x12 random damage OR 16 AoE damage.
/// Upgrade: +4 base damage to both modes.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class IlsaBrutalDrillSergeant : sts2_char_portalcraftCard
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
