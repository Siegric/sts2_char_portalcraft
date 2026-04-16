using BaseLib.Config;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode;

public class PortalcraftConfig : SimpleModConfig
{
    [ConfigSection("Audio")]
    [ConfigSlider(-20, 6, 1, Format = "{0} dB")]
    [ConfigHoverTip]
    public static double CardSfxVolume { get; set; } = -6;
}
