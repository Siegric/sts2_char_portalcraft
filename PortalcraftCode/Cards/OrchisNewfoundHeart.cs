using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class OrchisNewfoundHeart : PortalcraftCard
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
        var lloyd = CombatState.CreateCard<Lloyd>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(lloyd, PileType.Hand, addedByPlayer: true);
        
        await EnhancedPuppet.CreateInHand(Owner, 2, CombatState);
        
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
