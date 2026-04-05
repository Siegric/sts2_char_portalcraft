using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Devotee of Destruction — 2 cost Attack, Common.
/// Exhaust all cards in your hand. Get 2X Block and deal 2X damage.
/// X = number of cards exhausted this turn.
/// Upgrade: 3X instead of 2X.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class DevoteeOfDestruction : sts2_char_portalcraftCard
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Multiplier", 2m),
    };

    public DevoteeOfDestruction() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // Exhaust all cards in hand (except self, which is already being played)
        var handCards = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != this)
            .ToList();

        int exhaustedCount = 0;
        foreach (var card in handCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
            exhaustedCount++;
        }

        int totalExhausted = exhaustedCount;

        int multiplier = (int)DynamicVars["Multiplier"].BaseValue;
        int x = totalExhausted;

        // Gain 2X (or 3X) Block
        int blockAmount = multiplier * x;
        if (blockAmount > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, blockAmount, ValueProp.Move, cardPlay);
        }

        // Deal 2X (or 3X) damage
        int damageAmount = multiplier * x;
        if (damageAmount > 0)
        {
            await DamageCmd.Attack(damageAmount)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Multiplier"].UpgradeValueBy(1m);
    }
}
