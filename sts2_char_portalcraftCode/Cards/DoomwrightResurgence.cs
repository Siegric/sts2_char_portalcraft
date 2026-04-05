using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class DoomwrightResurgence : sts2_char_portalcraftCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public DoomwrightResurgence() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "DOOMWRIGHT_RESURGENCE_PROMPT"),
            minCount: 1,
            maxCount: 2
        );

        bool Filter(CardModel c) => c != this && c is ArtifactCard && c.EnergyCost.Canonical <= 1;

        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this);
        var selectedList = selected.ToList();

        foreach (var card in selectedList)
        {
            var copy = ArtifactHelper.CreateByType(card.GetType(), CombatState, Owner);
            copy.EnergyCost.SetThisTurnOrUntilPlayed(0, reduceOnly: true);
            copy.AddKeyword(CardKeyword.Ethereal);
            await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
