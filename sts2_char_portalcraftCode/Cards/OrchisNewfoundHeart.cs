using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Orchis, Newfound Heart — 2 cost Skill, Exhaust.
/// Add a Lloyd to your hand. Add 2 Enhanced Puppets to your hand.
/// Apply Recast 1 to all Puppets in your hand and exhaust pile.
/// Upgrade: remove Exhaust.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class OrchisNewfoundHeart : sts2_char_portalcraftCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Lloyd>(),
        HoverTipFactory.FromCard<EnhancedPuppet>(),
    };

    public OrchisNewfoundHeart() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Add Lloyd
        var lloyd = CombatState.CreateCard<Lloyd>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(lloyd, PileType.Hand, addedByPlayer: true);

        // Add 2 Enhanced Puppets
        await EnhancedPuppet.CreateInHand(Owner, 2, CombatState);

        // Apply Recast 1 to all Puppets in hand and exhaust pile
        var puppetsInHand = PileType.Hand.GetPile(Owner).Cards
            .Where(c => PuppetHelper.IsPuppet(c))
            .ToList();

        var puppetsInExhaust = PileType.Exhaust.GetPile(Owner).Cards
            .Where(c => PuppetHelper.IsPuppet(c))
            .ToList();

        foreach (var puppet in puppetsInHand.Concat(puppetsInExhaust))
        {
            puppet.BaseReplayCount += 1;
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
