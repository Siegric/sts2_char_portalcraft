using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Zwei, Symphonic Heart — 2 cost Skill.
/// Add a Victoria to your hand. Victoria gets Recast 1.
/// Upgrade: cost 1.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class ZweiSymphonicHeart : sts2_char_portalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Victoria>(),
    };

    public ZweiSymphonicHeart() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Add Victoria with Recast 1
        var victoria = CombatState.CreateCard<Victoria>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(victoria, PileType.Hand, addedByPlayer: true);
        victoria.BaseReplayCount += 1;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
