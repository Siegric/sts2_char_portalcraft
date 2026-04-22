using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Cards;

[Pool(typeof(PortalcraftCardPool))]
public sealed class LunarBunny : PortalcraftCard
{
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (CombatState == null) return false;
            return EnergyCost.GetResolved() < EnergyCost.Canonical;
        }
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(7m, ValueProp.Move),
    };

    public LunarBunny() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || IsClone) return Task.CompletedTask;

        int count = CombatManager.Instance.History.CardPlaysFinished
            .Count(e => e.CardPlay.Card.Type == CardType.Skill
                     && e.CardPlay.Card.Owner == Owner
                     && e.CardPlay.Card != this
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
        if (cardPlay.Card == this) return Task.CompletedTask;
        if (cardPlay.Card.Type != CardType.Skill) return Task.CompletedTask;

        EnergyCost.AddThisTurn(-1);
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
