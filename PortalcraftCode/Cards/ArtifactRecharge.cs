using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class ArtifactRecharge : PortalcraftCard, ITranscendenceCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<GearOfRemembrance>(),
        HoverTipFactory.FromCard<GearOfAmbition>(),
    };

    public ArtifactRecharge() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var gear1 = CombatState.CreateCard<GearOfRemembrance>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(gear1, PileType.Hand, addedByPlayer: true);

        var gear2 = CombatState.CreateCard<GearOfAmbition>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(gear2, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Biofabrication>();
    }
}
