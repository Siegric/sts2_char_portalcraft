using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class CarneliaEmberOfDarkness : sts2_char_portalcraftCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<GearOfRemembrance>(),
    };

    public CarneliaEmberOfDarkness() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var token = CombatState.CreateCard<GearOfRemembrance>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(token, PileType.Hand, addedByPlayer: true);

        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "CARNELIA_PROMPT"),
            minCount: 0,
            maxCount: 1
        );

        bool Filter(CardModel c) => c != this && c is ArtifactCard;

        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this);
        var card = selected.FirstOrDefault();

        if (card != null)
        {
            int energyGain = card.EnergyCost.Canonical + 1;
            await CardCmd.Exhaust(choiceContext, card);
            await PlayerCmd.GainEnergy(energyGain, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
