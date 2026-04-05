using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

/// <summary>
/// Ludicrous Ordnance — 1 cost Attack.
/// Add 2 copies of this card to your hand. Deal 3 damage to ALL enemies for each copy in hand.
/// Upgrade: +2 damage, +1 copy.
/// </summary>
[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class LudicrousOrdnance : sts2_char_portalcraftCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, ValueProp.Move),
        new CardsVar(2),
    };

    public LudicrousOrdnance() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Add copies to hand first
        int copies = (int)DynamicVars.Cards.BaseValue;
        for (int i = 0; i < copies; i++)
        {
            var copy = CombatState.CreateCard<LudicrousOrdnance>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
        }

        // Count all copies of this card in hand
        int countInHand = PileType.Hand.GetPile(Owner).Cards
            .Count(c => c is LudicrousOrdnance);

        // Deal damage to all enemies for each copy
        var enemies = CombatState.HittableEnemies;
        for (int i = 0; i < countInHand; i++)
        {
            foreach (var enemy in enemies)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(enemy)
                    .Execute(choiceContext);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
