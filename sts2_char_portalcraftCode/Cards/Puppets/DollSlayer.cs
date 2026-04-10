using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;

public sealed class DollSlayer : sts2_char_portalcraftCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { PuppetTag.Puppet };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(0m, ValueProp.Move),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public DollSlayer() : base(0, PuppetType.Puppet, CardRarity.Token, TargetType.AllEnemies, showInCardLibrary: true) { }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);

        // Add a fresh Vier, Heart Slayer to hand (inflates combat deck)
        var vier = CombatState.CreateCard<VierHeartSlayer>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(vier, PileType.Hand, addedByPlayer: true);
    }
}
