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
/// Myriad Designs — 2 cost Rare Skill, Exhaust.
/// Add a Ludicrous Ordnance, Shoddy Plaything, and Substandard Puppet to your hand.
/// They cost 0 this turn.
/// Upgrade: Add upgraded versions instead.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class MyriadDesigns : sts2_char_portalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<LudicrousOrdnance>(),
        HoverTipFactory.FromCard<ShoddyPlaything>(),
        HoverTipFactory.FromCard<SubstandardPuppet>(),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public MyriadDesigns() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var ordnance = CombatState.CreateCard<LudicrousOrdnance>(Owner);
        var plaything = CombatState.CreateCard<ShoddyPlaything>(Owner);
        var puppet = CombatState.CreateCard<SubstandardPuppet>(Owner);

        if (IsUpgraded)
        {
            CardCmd.Upgrade(ordnance);
            CardCmd.Upgrade(plaything);
            CardCmd.Upgrade(puppet);
        }

        ordnance.EnergyCost.SetThisTurnOrUntilPlayed(0, reduceOnly: true);
        plaything.EnergyCost.SetThisTurnOrUntilPlayed(0, reduceOnly: true);
        puppet.EnergyCost.SetThisTurnOrUntilPlayed(0, reduceOnly: true);

        await CardPileCmd.AddGeneratedCardsToCombat(
            new CardModel[] { ordnance, plaything, puppet }, PileType.Hand, addedByPlayer: true);
    }
}
