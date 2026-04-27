using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;
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

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),
    };

    public UnfeelingEldAxe() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    
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
        var candidates = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != this
                     && c is IEvolvableCard
                     && c.EnergyCost.Canonical >= 2
                     && EvoRuntime.GetTier(c) == null)
            .ToList();

        if (candidates.Count == 0) return;

        var picked = Owner.RunState.Rng.Shuffle.NextItem(candidates);
        await EvoCmd.ForceEvolve(picked, choiceContext, playVfx: true);

        var enemies = CombatState.HittableEnemies.ToList();
        if (enemies.Count == 0) return;

        var target = Owner.RunState.Rng.Shuffle.NextItem(enemies);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
