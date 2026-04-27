using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace sts2_char_portalcraft.PortalcraftCode.GameActions;

public struct Sts2CharPortalcraft_NetEvolveAction : INetAction, IPacketSerializable
{
    public bool isSuper;
    public NetCombatCard target;

    public GameAction ToGameAction(Player player)
        => new Sts2CharPortalcraft_EvolveAction(player, isSuper, target);

    public void Serialize(PacketWriter writer)
    {
        writer.WriteBool(isSuper);
        writer.Write(target);
    }

    public void Deserialize(PacketReader reader)
    {
        isSuper = reader.ReadBool();
        target = reader.Read<NetCombatCard>();
    }

    public override string ToString()
        => $"Sts2CharPortalcraft_NetEvolveAction (isSuper: {isSuper}, target: {target})";
}
