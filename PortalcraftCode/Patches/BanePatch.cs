using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.Patches;

[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.Damage),
    new[] { typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel) })]
public static class BaneDamagePatch
{
    public static void Prefix(ref decimal amount, IEnumerable<Creature> targets, CardModel? cardSource)
    {
        if (cardSource == null) return;
        if (!cardSource.Keywords.Contains(BaneKeyword.Bane)) return;
        
        foreach (var target in targets)
        {
            if (target == null) continue;
            if (target.CurrentHp <= 0m) continue;
            if (!target.HasPower<MinionPower>())
            {
                amount *= 2m;
                break;
            }
        }
    }

    public static void Postfix(IEnumerable<Creature> targets, CardModel? cardSource)
    {
        if (cardSource == null) return;
        if (!cardSource.Keywords.Contains(BaneKeyword.Bane)) return;

        foreach (var target in targets)
        {
            if (target == null) continue;
            if (target.CurrentHp <= 0m) continue;
            if (target.HasPower<MinionPower>())
            {
                target.SetCurrentHpInternal(0m);
            }
        }
    }
}
