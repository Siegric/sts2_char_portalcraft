using BaseLib.Config;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode;

public class PortalcraftConfig : SimpleModConfig
{
    [ConfigSection("Audio")]
    [SliderRange(-20, 6, 1)]
    [SliderLabelFormat("{0} dB")]
    [ConfigHoverTip]
    public static double CardSfxVolume { get; set; } = -6;
}
