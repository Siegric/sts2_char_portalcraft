using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Omen;

public sealed class WhitePsalmNewRevelation : sts2_char_portalcraftCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { OmenTag.Talisman };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Retain,
        CardKeyword.Unplayable,
    };

    public WhitePsalmNewRevelation() : base(0, TalismanType.Talisman, CardRarity.Token, TargetType.Self) { }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    protected override void OnUpgrade() { }

    public static async Task<CardModel> CreateInHand(Player owner, CombatState combatState)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return null;

        var psalm = combatState.CreateCard<WhitePsalmNewRevelation>(owner);
        await CardPileCmd.AddGeneratedCardToCombat(psalm, PileType.Hand, addedByPlayer: true);
        await TalismanHelper.EnsureTalismanPower(owner, psalm);
        return psalm;
    }
}
