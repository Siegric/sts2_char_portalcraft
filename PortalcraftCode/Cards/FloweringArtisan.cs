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
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Character;
using sts2_char_portalcraft.PortalcraftCode.Powers;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class FloweringArtisan : PortalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 5m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<FloweringArtisanPower>(),
    };

    public FloweringArtisan() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var skills = PileType.Draw.GetPile(Owner).Cards.Where(c => c.Type == CardType.Skill).ToList();
        if (skills.Count > 0)
        {
            var prefs = new CardSelectorPrefs(
                new LocString("card_selection", "FLOWERING_ARTISAN_PROMPT"),
                minCount: 1,
                maxCount: 1);

            var chosen = (await CardSelectCmd.FromSimpleGrid(choiceContext, skills, Owner, prefs)).ToList();
            if (chosen.Count > 0)
            {
                await CardPileCmd.Add(chosen[0], PileType.Hand);
            }
        }
        
        int damage = (int)DynamicVars["MagicNumber"].BaseValue;
        await PowerCmd.Apply<FloweringArtisanPower>(Owner.Creature, damage, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(2m);
    }
}
