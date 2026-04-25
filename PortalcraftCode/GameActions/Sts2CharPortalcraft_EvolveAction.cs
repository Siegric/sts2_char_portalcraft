using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using sts2_char_portalcraft.PortalcraftCode.Cards.Keywords;

namespace sts2_char_portalcraft.PortalcraftCode.GameActions;

public sealed class Sts2CharPortalcraft_EvolveAction : GameAction
{
    public Player Player { get; }
    public bool IsSuper { get; }
    public NetCombatCard Target { get; }
    public PlayerChoiceContext? PlayerChoiceContext { get; private set; }

    public override ulong OwnerId => Player.NetId;
    public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

    public Sts2CharPortalcraft_EvolveAction(Player player, bool isSuper, NetCombatCard target)
    {
        Player = player;
        IsSuper = isSuper;
        Target = target;
    }

    protected override async Task ExecuteAction()
    {
        var card = Target.ToCardModelOrNull();
        if (card == null) return;
        if (card.Owner != Player) return;

        PlayerChoiceContext = new GameActionPlayerChoiceContext(this);
        if (IsSuper)
        {
            if (!EvoCmd.CanSuperEvolve(card)) return;
            await EvoCmd.TrySuperEvolve(card, PlayerChoiceContext);
        }
        else
        {
            if (!EvoCmd.CanEvolve(card)) return;
            await EvoCmd.TryEvolve(card, PlayerChoiceContext);
        }
    }

    public override INetAction ToNetAction() => new Sts2CharPortalcraft_NetEvolveAction
    {
        isSuper = IsSuper,
        target = Target,
    };

    public override string ToString()
        => $"Sts2CharPortalcraft_EvolveAction (isSuper: {IsSuper}, target: {Target})";
}
