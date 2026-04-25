using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Omen;

public sealed class WastelandOfDestructionToken : PortalcraftCard, ILastWordsCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { OmenTag.Amulet, OmenTag.WastelandToken };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Retain,
        CardKeyword.Unplayable,
        LastWordsKeyword.LastWords,
    };

    public WastelandOfDestructionToken() : base(0, AmuletType.Amulet, CardRarity.Token, TargetType.Self, showInCardLibrary: true) { }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    protected override void OnUpgrade() { }

    public async Task OnLastWords(PlayerChoiceContext choiceContext)
    {
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }

    public static async Task<CardModel> CreateInHand(Player owner, ICombatState combatState)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return null;

        var token = combatState.CreateCard<WastelandOfDestructionToken>(owner);
        await CardPileCmd.AddGeneratedCardToCombat(token, PileType.Hand, owner);
        return token;
    }
}
