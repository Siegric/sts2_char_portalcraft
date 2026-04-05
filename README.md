# Portalcraft - Slay the Spire 2 Character Mod

A custom character mod for Slay the Spire 2, inspired by Portalcraft from Shadowverse.

### Archetype Implementation

**Constructs** — Token merge system. `ConstructCard` base class with `ConstructTier` (T0-T3) and `MergeRecipes` defining upgrade paths. `IMergeListener` interface allows cards to react to merge events.

**Puppets** — Expendable token generation. `PuppetTag` identifies tokens, `PuppetHelper` handles creation. Powers like `LiamCrazedCreatorPower` and `ReplayNextPuppetPower` provide scaling.

**Omen of Destruction** — Exhaust-driven Talisman cycle. `TalismanPower` (hidden, `PowerStackType.Single`) manages all Psalm end-of-turn effects, transformation, and exhaust triggers via `BeforeTurnEnd` and `AfterCardExhausted` hooks. `OmenTag.Talisman` and `OmenTag.WastelandToken` are custom `CardTag`s. Psalms use `CardKeyword.Retain` + `CardKeyword.Unplayable`. `BeelzebubSupremeKingPower` is a marker power checked by `TalismanPower` to double outputs. `AxiaHeirToDestructionPower` exhausts all Talismans at end of turn via `CardCmd.Exhaust`, which triggers `AfterCardExhausted` hooks for double activation.

### Project Structure

```
sts2_char_portalcraftCode/
  Audio/
    CardPlayAudioManager.cs         # Godot AudioStreamPlayer manager
  Cards/
    Constructs/                     # Merge pyramid cards + ConstructCard base
    Omen/                           # Talisman tokens + OmenTag + TalismanHelper
    Puppets/                        # Puppet tokens + PuppetTag + PuppetHelper
    sts2_char_portalcraftCard.cs    # Base card class (portrait path logic)
  Character/
    sts2_char_portalcraft.cs        # Character definition + starting deck
  Extensions/
    StringExtensions.cs             # Asset path helpers (CardImagePath, etc.)
  Patches/
    CardPlayAudioPatch.cs           # Harmony patches for card audio
  Powers/                           # All power implementations
  Relics/
    ResonanceCore.cs                # Starting relic
  MainFile.cs                       # ModInitializer + Harmony.PatchAll + audio init

sts2_char_portalcraft/              # Resource directory (packed into .pck)
  audio/                            # .ogg voice lines (named by class)
  images/
    card_portraits/                 # Normal (500x380) + big/ (1000x760)
    charui/                         # Character UI assets
    powers/ + relics/               # Icon assets
  localization/
    eng/ + zhs/                     # JSON localization files
```

**Prerequisites:** .NET 9.0 SDK, Godot 4.5.1 Mono, Slay the Spire 2, BaseLib NuGet package.

---