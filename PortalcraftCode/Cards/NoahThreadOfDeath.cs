using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using sts2_char_portalcraft.PortalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class NoahThreadOfDeath : PortalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 4m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<Puppet>(),
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public NoahThreadOfDeath() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int bonus = (int)DynamicVars["MagicNumber"].BaseValue;

        var puppets = new List<CardModel>();
        for (int i = 0; i < 3; i++)
            puppets.Add(CombatState.CreateCard<Puppet>(Owner));
        await CardPileCmd.AddGeneratedCardsToCombat(puppets, PileType.Hand, Owner);

        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (!PuppetHelper.IsPuppet(card)) continue;
            card.AddKeyword(CardKeyword.Retain);
            if (card is Lloyd) continue;
            card.DynamicVars.Damage.BaseValue += bonus;
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
