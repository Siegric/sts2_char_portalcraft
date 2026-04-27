using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Cards.Omen;

public sealed class WhitePsalmNewRevelation : PortalcraftCard, ICountdownCard, ILastWordsCard
{
    private const int BaseBlockAmount = 4;
    private const int UpgradeBlockAmount = 1;

    public int BlockValue => BaseBlockAmount + (CurrentUpgradeLevel > 0 ? UpgradeBlockAmount : 0);

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
        new IntVar(CountdownHelper.CountdownVarName, 1m),
    };

    public WhitePsalmNewRevelation() : base(0, AmuletType.Amulet, CardRarity.Token, TargetType.Self, showInCardLibrary: true) { }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("PsalmBlock", BlockValue.ToString());
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    protected override void OnUpgrade() { }

    public async Task OnLastWords(PlayerChoiceContext choiceContext)
    {
        int block = BlockValue + AmuletHelper.GetBeelzebubBonus(Owner.Creature);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Unpowered, null);

        var black = CombatState.CreateCard<BlackPsalmNewRevelation>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(black, PileType.Hand, Owner);
    }

    public static async Task<CardModel> CreateInHand(Player owner, ICombatState combatState)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return null;

        var psalm = combatState.CreateCard<WhitePsalmNewRevelation>(owner);
        await CardPileCmd.AddGeneratedCardToCombat(psalm, PileType.Hand, owner);
        return psalm;
    }
}
