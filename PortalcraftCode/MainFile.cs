using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using sts2_char_portalcraft.PortalcraftCode.Audio;

namespace sts2_char_portalcraft.PortalcraftCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "sts2_char_portalcraft"; //Used for resource filepath

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static PortalcraftConfig Config { get; private set; }

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        Config = new PortalcraftConfig();
        ModConfigRegistry.Register(ModId, Config);
        Config.ConfigChanged += (_, _) => CardPlayAudioManager.SetVolume((float)PortalcraftConfig.CardSfxVolume);
        CardPlayAudioManager.SetVolume((float)PortalcraftConfig.CardSfxVolume);

        CardPlayAudioManager.Initialize();
    }
}