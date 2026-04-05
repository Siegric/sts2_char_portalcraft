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
/// Sylvia, Garden Executioner — 1 cost Skill.
/// Choose a Draw Cards or Heal card to add to your hand. Upgrade: cost -1.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class SylviaGardenExecutioner : sts2_char_portalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<SylviaDrawChoice>(),
        HoverTipFactory.FromCard<SylviaHealChoice>(),
    };

    public SylviaGardenExecutioner() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawCard = CombatState.CreateCard<SylviaDrawChoice>(Owner);
        var healCard = CombatState.CreateCard<SylviaHealChoice>(Owner);

        var cards = new List<CardModel> { drawCard, healCard };
        var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner);

        if (chosen != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
