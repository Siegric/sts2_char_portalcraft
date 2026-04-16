using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Cards;

[Pool(typeof(sts2_char_portalcraftCardPool))]
public sealed class ShoRebornNightKing : sts2_char_portalcraftCard
{
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (CombatState == null) return false;
            int skillsPlayed = CombatManager.Instance.History.CardPlaysFinished
                .Count(e => e.CardPlay.Card.Type == CardType.Skill
                         && e.CardPlay.Card.Owner == Owner
                         && e.CardPlay.Card != this
                         && e.HappenedThisTurn(CombatState));
            return skillsPlayed >= (int)DynamicVars["SkillThreshold"].BaseValue;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MagicNumber", 3m),
        new IntVar("SkillThreshold", 3m),
    };

    public ShoRebornNightKing() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int skillsPlayed = CombatManager.Instance.History.CardPlaysFinished
            .Count(e => e.CardPlay.Card.Type == CardType.Skill
                     && e.CardPlay.Card.Owner == Owner
                     && e.CardPlay.Card != this
                     && e.HappenedThisTurn(CombatState));

        if (skillsPlayed >= (int)DynamicVars["SkillThreshold"].BaseValue)
        {
            await PlayerCmd.GainEnergy(DynamicVars["MagicNumber"].BaseValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
