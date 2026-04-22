using BaseLib.Abstracts;
using BaseLib.Utils;
using sts2_char_portalcraft.PortalcraftCode.Character;

namespace sts2_char_portalcraft.PortalcraftCode.Potions;

[Pool(typeof(PortalcraftPotionPool))]
public abstract class PortalcraftPotion : CustomPotionModel;