using BaseLib.Abstracts;
using sts2_char_portalcraft.sts2_char_portalcraftCode.Extensions;
using Godot;

namespace sts2_char_portalcraft.sts2_char_portalcraftCode.Character;

public class sts2_char_portalcraftRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => sts2_char_portalcraft.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}