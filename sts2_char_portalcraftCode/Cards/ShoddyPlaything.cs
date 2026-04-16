using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class ShoddyPlaything : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(3),
    };

    public ShoddyPlaything() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, (int)DynamicVars.Cards.BaseValue, Owner);

        var copyDraw = CombatState.CloneCard(this);
        await CardPileCmd.AddGeneratedCardToCombat(copyDraw, PileType.Draw, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
