using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.PortalcraftCode.Extensions;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Omen;

public sealed class PuppetTheaterToken : PortalcraftCard, ICountdownCard, IOnTurnStartCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { OmenTag.Amulet };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Retain,
        CardKeyword.Unplayable,
        CountdownKeyword.Countdown,
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar(CountdownHelper.CountdownVarName, 2m),
        new IntVar("MagicNumber", 1m),
    };

    public PuppetTheaterToken()
        : base(0, AmuletType.Amulet, CardRarity.Token, TargetType.Self, showInCardLibrary: true) { }

    public override string PortraitPath       => "puppet_theater.png".CardImagePath();
    public override string CustomPortraitPath => "puppet_theater.png".BigCardImagePath();

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        => Task.CompletedTask;

    public async Task OnTurnStart(PlayerChoiceContext choiceContext)
    {
        int amount = (int)DynamicVars["MagicNumber"].BaseValue;
        await Puppet.CreateInHand(Owner, amount, CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(1m);
    }

    public static async Task<CardModel> CreateInHand(Player owner, CombatState combatState, bool upgraded)
    {
        if (CombatManager.Instance.IsOverOrEnding) return null;
        var token = combatState.CreateCard<PuppetTheaterToken>(owner);
        if (upgraded) token.UpgradeInternal();
        await CardPileCmd.AddGeneratedCardToCombat(token, PileType.Hand, true);
        return token;
    }
}
