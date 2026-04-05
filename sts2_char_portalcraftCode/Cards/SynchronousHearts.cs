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
/// Synchronous Hearts — 2 cost Skill, Exhaust.
/// Add a Lloyd and a Victoria to your hand.
/// Upgrade: remove Exhaust.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class SynchronousHearts : sts2_char_portalcraftCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Lloyd>(),
        HoverTipFactory.FromCard<Victoria>(),
    };

    public SynchronousHearts() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var lloyd = CombatState.CreateCard<Lloyd>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(lloyd, PileType.Hand, addedByPlayer: true);

        var victoria = CombatState.CreateCard<Victoria>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(victoria, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
