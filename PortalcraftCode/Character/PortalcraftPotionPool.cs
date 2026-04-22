using BaseLib.Abstracts;
using sts2_char_portalcraft.PortalcraftCode.Extensions;
using Godot;

namespace sts2_char_portalcraft.PortalcraftCode.Character;

public class PortalcraftPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Portalcraft.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}