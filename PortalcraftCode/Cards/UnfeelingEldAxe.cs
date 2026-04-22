using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class UnfeelingEldAxe : PortalcraftCard
{
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (CombatState == null) return false;
            return EnergyCost.GetResolved() < EnergyCost.Canonical;
        }
    }

    public UnfeelingEldAxe() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || IsClone) return Task.CompletedTask;

        int count = CombatManager.Instance.History.CardPlaysFinished
            .Count(e => e.CardPlay.Card.EnergyCost.Canonical >= 2
                     && e.CardPlay.Card.Owner == Owner
                     && e.HappenedThisTurn(CombatState));
        if (count > 0)
        {
            EnergyCost.AddThisTurn(-count);
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner) return Task.CompletedTask;
        if (cardPlay.Card.EnergyCost.Canonical < 2) return Task.CompletedTask;

        EnergyCost.AddThisTurn(-1);
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handCards = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        if (handCards.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "ELD_AXE_REDUCE_PROMPT"),
            minCount: 1,
            maxCount: 1);

        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, c => c != this, this);
        var card = selected.FirstOrDefault();
        if (card != null)
        {
            card.EnergyCost.AddThisCombat(-1);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
