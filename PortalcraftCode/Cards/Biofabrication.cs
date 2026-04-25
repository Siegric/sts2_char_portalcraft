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
using sts2_char_portalcraft.PortalcraftCode.Cards.Artifacts;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class Biofabrication : PortalcraftCard
{
    public Biofabrication() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "BIOFABRICATION_PROMPT"),
            minCount: 1,
            maxCount: 1);

        bool Filter(CardModel c) => c != this && c is ArtifactCard;

        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, Filter, this)).ToList();
        if (selected.Count == 0) return;

        var chosen = selected[0];
        
        chosen.EnergyCost.AddThisCombat(-1);
        
        for (int i = 0; i < 3; i++)
        {
            var copy = ArtifactHelper.CreateByType(chosen.GetType(), CombatState, Owner);
            copy.EnergyCost.AddThisCombat(-1);
            await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Draw, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
