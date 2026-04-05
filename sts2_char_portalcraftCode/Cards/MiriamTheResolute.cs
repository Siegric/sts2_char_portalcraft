using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class MiriamTheResolute : sts2_char_portalcraftCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<GearOfRemembrance>(),
        HoverTipFactory.FromCard<GearOfAmbition>(),
    };

    public MiriamTheResolute() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (int i = 0; i < 2; i++)
        {
            var gear = CombatState.CreateCard<GearOfRemembrance>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(gear, PileType.Hand, addedByPlayer: true);
        }
        for (int i = 0; i < 2; i++)
        {
            var gear = CombatState.CreateCard<GearOfAmbition>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(gear, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
