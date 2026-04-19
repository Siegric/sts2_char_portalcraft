using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Keywords;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Omen;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Cards.Puppets;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class PuppetTheater : sts2_char_portalcraftCard, ICountdownCard, IOnTurnStartCard
{
    protected override HashSet<CardTag> CanonicalTags => new() { OmenTag.Amulet };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Retain,
        CardKeyword.Unplayable,
        CountdownKeyword.Countdown,
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar(CountdownHelper.CountdownVarName, 2m),
        new IntVar("MagicNumber", 1m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(CountdownKeyword.Countdown),
        HoverTipFactory.FromCard<Puppet>(),
    };

    public PuppetTheater() : base(1, AmuletType.Amulet, CardRarity.Uncommon, TargetType.Self) { }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public async Task OnTurnStart(PlayerChoiceContext choiceContext)
    {
        int amount = (int)DynamicVars["MagicNumber"].BaseValue;
        await Puppet.CreateInHand(Owner, amount, CombatState);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MagicNumber"].UpgradeValueBy(1m);
    }
}
