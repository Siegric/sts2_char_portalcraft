using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class RukinaResistanceLeader : PortalcraftCard
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(8m, ValueProp.Move),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<GearOfAmbition>(),
        HoverTipFactory.FromCard<GearOfRemembrance>(),
        HoverTipFactory.FromCard<StrikerArtifact>(),
    };

    public RukinaResistanceLeader() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        var gear1 = CombatState.CreateCard<GearOfAmbition>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(gear1, PileType.Hand, addedByPlayer: true);

        var gear2 = CombatState.CreateCard<GearOfRemembrance>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(gear2, PileType.Hand, addedByPlayer: true);

        var striker = CombatState.CreateCard<StrikerArtifact>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(striker, PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
