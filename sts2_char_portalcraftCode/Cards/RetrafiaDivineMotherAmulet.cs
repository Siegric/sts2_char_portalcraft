using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Omen;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

public sealed class RetrafiaDivineMotherAmulet : sts2_char_portalcraftCard, ICountdownCard, ILastWordsCard
{
    private const int BaseCountdown = 10;

    protected override HashSet<CardTag> CanonicalTags => new() { OmenTag.Amulet };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Retain,
        CardKeyword.Unplayable,
        CountdownKeyword.Countdown,
        LastWordsKeyword.LastWords,
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar(CountdownHelper.CountdownVarName, BaseCountdown),
    };

    public RetrafiaDivineMotherAmulet()
        : base(0, AmuletType.Amulet, CardRarity.Token, TargetType.Self, showInCardLibrary: true) { }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    protected override void OnUpgrade() { }

    public async Task OnLastWords(PlayerChoiceContext choiceContext)
    {
        if (CombatManager.Instance.IsOverOrEnding) return;

        var retrafia = CombatState.CreateCard<RetrafiaDivineMother>(Owner);
        retrafia.EnergyCost.SetThisTurnOrUntilPlayed(0, reduceOnly: true);
        await CardPileCmd.AddGeneratedCardToCombat(retrafia, PileType.Hand, addedByPlayer: true);
    }
}
