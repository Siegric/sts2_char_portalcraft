using BaseLib.Abstracts;
using BaseLib.Extensions;
using sts2_char_portalcraft.PortalcraftCode.Extensions;
using Godot;

namespace sts2_char_portalcraft.PortalcraftCode.Powers;

public abstract class PortalcraftPower : CustomPowerModel
{
    //Loads from sts2_char_portalcraft/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}