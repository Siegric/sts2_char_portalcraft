# Portalcraft — Custom API Reference

Slay the Spire 2 character mod inspired by Shadowverse's Portalcraft.

**Prerequisites:** .NET 9.0 SDK, Godot 4.5.1 Mono, Slay the Spire 2, BaseLib NuGet.

---

## Systems 

| System | Entry point | Purpose |
|---|---|---|
| Evolution | `EvoCmd` | Spend EP / SEP to buff cards in hand |
| Artifact Fusion | `ArtifactCard` + `FuseRecipes` | Combine artifacts into higher-tier ones |
| Countdown | `ICountdownCard` + `CountdownHelper` | Cards tick down each turn; exhaust at 0 |
| Crystallize | `ICrystallizeCard` + `CrystallizeRuntime` | Play for a discount → turns into Amulet |
| Last Words | `ILastWordsCard` / `ILastWordsEnchantment` | Effect fires on exhaust |
| On-Turn-Start | `IOnTurnStartCard` | Fires at the start of the player's turn |
| Skybound Art | `SkyboundArt` keyword (placeholder) | Gauge-based ability (unimplemented) |
| Super Skybound Art | `SuperSkyboundArt` keyword (placeholder) | Gauge ≥15 variant (unimplemented) |

All custom keyword dispatching flows through **`KeywordDispatcherPower`** — a
hidden power applied to the player at combat start by `ResonanceCore`.

---

## Evolution

Player spends 1 of 2 EP or 1 of 2 SEP per combat to buff an in-hand card.
Unused points carry between turns; only one evolution action per turn; points
don't regenerate. Default buff: +2 damage / +2 block (evolve) or +3 / +3
(super-evolve), applied passively via `KeywordDispatcherPower`.

### Authoring an evolvable card

```csharp
public sealed class MyCard : sts2_char_portalcraftCard, IEvolvableCard
{
    public Task OnEvolve(CardModel card, PlayerChoiceContext ctx)      => Task.CompletedTask;
    public Task OnSuperEvolve(CardModel card, PlayerChoiceContext ctx) => Task.CompletedTask;
}
```

Evolved cards automatically glow gold using `ShouldGlowGoldInternal`

### IEvolvableCard

- `Task OnEvolve(CardModel card, PlayerChoiceContext ctx)`
  — Card's custom evolve effect; default is no-op (passive stat boost only).
- `Task OnSuperEvolve(CardModel card, PlayerChoiceContext ctx)`
  — Card's custom super-evolve effect; default is no-op.

### EvoCmd 

Player-initiated:

- `bool EvoCmd.CanEvolve(CardModel card)`
  — True if the player has 1+ EP, isn't turn-locked, and the card is an un-evolved `IEvolvableCard`.
- `bool EvoCmd.CanSuperEvolve(CardModel card)`
  — Same for SEP; allows re-tier on an already-evolved card (tier ≠ SuperEvolved).
- `Task<bool> EvoCmd.TryEvolve(CardModel card, PlayerChoiceContext ctx)`
  — Spends 1 EP, runs the full pipeline (mark → glow → in-hand VFX → OnEvolve).
- `Task<bool> EvoCmd.TrySuperEvolve(CardModel card, PlayerChoiceContext ctx)`
  — Spends 1 SEP, runs the full pipeline (mark → glow → center-screen VFX → OnSuperEvolve).

Card-initiated:

- `bool EvoCmd.CanForceEvolve(CardModel card)`
  — True if the card is an `IEvolvableCard` with no tier yet.
- `bool EvoCmd.CanForceSuperEvolve(CardModel card)`
  — True if the card is an `IEvolvableCard` not already super-evolved.
- `Task<bool> EvoCmd.ForceEvolve(CardModel card, PlayerChoiceContext ctx, bool playVfx = true)`
  — Evolves without spending EP; set `playVfx=false` for self-evolve on play (avoids double animation).
- `Task<bool> EvoCmd.ForceSuperEvolve(CardModel card, PlayerChoiceContext ctx, bool playVfx = true)`
  — Super-evolves without spending SEP; same `playVfx` control.

Hand-level predicates:

- `bool EvoCmd.CanEvolveAny(Player player)`
  — True if the player could evolve *some* card in hand right now.
- `bool EvoCmd.CanSuperEvolveAny(Player player)`
  — Same for super-evolve; used to grey out the SEP holder.

High-level UI flows:

- `Task<bool> EvoCmd.EvolveFromHand(Player p, PlayerChoiceContext ctx)`
  — Opens the modal `NPlayerHand.SelectCards` picker, then applies `TryEvolve` on the pick.
- `Task<bool> EvoCmd.EvolveFromHandWithArrow(Player p, PlayerChoiceContext ctx, Vector2 arrowStart)`
  — Starts drag-to-target arrow from `arrowStart`, applies `TryEvolve` on release.
- `Task<bool> EvoCmd.SuperEvolveFromHand(Player p, PlayerChoiceContext ctx)` / `SuperEvolveFromHandWithArrow(...)`
  — SEP variants of the two flows above.

**Force-evolve patterns:**

```csharp
// Self-evolve on play (stat boost applies to this play)
protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
{
    await EvoCmd.ForceEvolve(this, ctx, playVfx: false);
    await DamageCmd.Attack(DynamicVars.Damage).FromCard(this).Targeting(play.Target!).Execute(ctx);
}

// Evolve a picked target on play
protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
{
    var prefs = new CardSelectorPrefs(new LocString("card_selection", "EVOLVE_TARGET_PROMPT"), 1);
    var pick = (await CardSelectCmd.FromHand(ctx, Owner, prefs, EvoCmd.CanForceEvolve, this))
                   .FirstOrDefault();
    if (pick != null) await EvoCmd.ForceEvolve(pick, ctx);
}
```

### EvoRuntime

Constants:

- `const int EvoRuntime.MaxEvoPoints = 2` — Starting EP per combat.
- `const int EvoRuntime.MaxSuperEvoPoints = 2` — Starting SEP per combat.
- `const decimal EvoRuntime.EvolveDamageBonus = 2` — Passive +dmg from Evolve.
- `const decimal EvoRuntime.EvolveBlockBonus = 2` — Passive +block from Evolve.
- `const decimal EvoRuntime.SuperEvolveDamageBonus = 3` — Passive +dmg from Super Evolve.
- `const decimal EvoRuntime.SuperEvolveBlockBonus = 3` — Passive +block from Super Evolve.
- `enum EvoRuntime.Tier { Evolved, SuperEvolved }` — Per-card evolution tier.

Pool:

- `void EvoRuntime.InitForCombat(PlayerCombatState pcs)`
  — Resets EP/SEP to max and clears tier dict; called by `ResonanceCore.BeforeCombatStart`.
- `int EvoRuntime.EvoPoints(PlayerCombatState pcs)` / `SuperEvoPoints(PlayerCombatState pcs)`
  — Read current point counts.
- `bool EvoRuntime.UsedThisTurn(PlayerCombatState pcs)`
  — True if any evolution action has been used this turn (shared lockout across tiers).
- `bool EvoRuntime.CanEvolve(PlayerCombatState pcs)` / `CanSuperEvolve(PlayerCombatState pcs)`
  — True if the corresponding point is available and the turn lockout isn't set.
- `bool EvoRuntime.TrySpendEvo(PlayerCombatState pcs)` / `TrySpendSuperEvo(PlayerCombatState pcs)`
  — Atomically decrements a point and sets the turn lockout; returns false if unavailable.
- `void EvoRuntime.ResetTurnLockout(PlayerCombatState pcs)`
  — Clears the turn lockout; called by `KeywordDispatcherPower.AfterPlayerTurnStart`.

Per-card tier:

- `Tier? EvoRuntime.GetTier(CardModel card)`
  — Returns the card's current tier, or null if not evolved.
- `bool EvoRuntime.IsEvolved(CardModel card)` / `IsSuperEvolved(CardModel card)`
  — Tier-specific checks; `IsEvolved` returns false if super-evolved.
- `void EvoRuntime.MarkEvolved(CardModel card)` / `MarkSuperEvolved(CardModel card)`
  — Sets the tier; overwrites any prior tier on the same card.

Events:

- `event Action<PlayerCombatState> EvoRuntime.Changed`
  — Fires on any point / lockout / tier mutation; wired to `NEvoHolder` for live UI refresh.

### Passive stat-boost hooks (KeywordDispatcherPower)

- `decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)`
  — Returns `EvolveDamageBonus` (or Super) when `cardSource` is an evolved card owned by this power's creature.
- `decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)`
  — Returns `EvolveBlockBonus` (or Super) under the same conditions.

### Files

| File | Role |
|---|---|
| `Cards/Keywords/EvoCmd.cs` | Command gateway — Try* and Force* entry points |
| `Cards/Keywords/EvoRuntime.cs` | Per-combat pool + per-card tier state |
| `Cards/Keywords/EvoTargeting.cs` | Session flag for arrow-select mode |
| `Cards/Keywords/IEvolvableCard.cs` | Opt-in tag interface |
| `UI/NEvoHolder.cs` | Button factory (staged icons, click-drag-arrow) |
| `Patches/EvoHolderInjectPatch.cs` | Injects holders into `NCombatRoom` |
| `Patches/NCardEvoTargetingPatch.cs` | Hand-hover bridge + play-click suppression + hover-tip suppression |

---

## Artifact Fusion

| Tier | Cards | Fuse rules |
|---|---|---|
| T0 | `GearOfAmbition`, `GearOfRemembrance` | Any T0 pairs with any T0. Played card determines result: Ambition → `StrikerArtifact`, Remembrance → `FortifierArtifact` |
| T1 | `StrikerArtifact`, `FortifierArtifact` | Consumes any mix of T0 / T1 / T2. Result by total energy of discards: 1→Alpha, 2→Beta, 3+→Gamma |
| T2 | `OminousArtifactAlpha`, `Beta`, `Gamma` | Requires the other two T2 types → `MasterworkOmega` |
| T3 | `MasterworkOmega` | Not fusable |

### Authoring an artifact

```csharp
public sealed class MyArtifact : ArtifactCard
{
    public override ArtifactTier Tier => ArtifactTier.T1;
    public MyArtifact() : base(energyCost: 1, ArtifactType.Artifact, TargetType.Self) { }
    protected override Task OnRawPlay(PlayerChoiceContext ctx, CardPlay play) { ... }
    public override Task ActivateEffect(PlayerChoiceContext ctx) { ... }   //out-of-hand activation
}
```

### ArtifactCard

- `abstract ArtifactTier Tier { get; }`
  — Declares which tier this artifact belongs to.
- `int FuseCost => 0`
  — Energy refunded on fuse; 0 means fusing is always free.
- `bool HasValidFusionPartnerInHand()`
  — True if at least one valid discard partner exists in hand right now.
- `protected abstract Task OnRawPlay(PlayerChoiceContext ctx, CardPlay play)`
  — Card's effect when played without fusing (or when no fusion partner is chosen).
- `virtual Task ActivateEffect(PlayerChoiceContext ctx)`
  — Card's effect when activated out of hand (e.g. by `RalmiaSonicRacer`); default no-op.

### FuseRecipes

- `Type? FuseRecipes.FindResult(CardModel playedCard, IReadOnlyList<CardModel> discardedCards)`
  — Returns the resulting artifact type for the given fusion, or null if no recipe matches.
- `HashSet<Type> FuseRecipes.GetValidDiscardTypes(Type playedType, ArtifactTier tier)`
  — Returns card types that are valid discard partners for a played card at `tier`.

### IFuseListener

- `Task OnArtifactFused(PlayerChoiceContext ctx, ArtifactCard playedCard, IReadOnlyList<CardModel> discardedCards, CardModel resultCard, ArtifactTier resultTier)`
  — Fires for every successful fusion on the played card's owner's active powers + relics.

Currently implemented by `AncientCannonPower`, `RalmiaSonicRacerPower`, `FusionPlating`.

### ArtifactHelper

- `Type[] ArtifactHelper.T0Types / T1Types / T2Types`
  — Static arrays of concrete card types per tier.
- `CardModel ArtifactHelper.CreateByType(Type type, CombatState state, Player owner)`
  — Instantiates an artifact by its runtime Type (convenience for reflection-driven code).
- `Task ArtifactHelper.AddRandomArtifacts(Type[] pool, int count, CombatState state, Player owner, Rng rng)`
  — Adds `count` random artifacts from `pool` directly into hand.
- `int ArtifactHelper.CountArtifactsInHand(Player owner)`
  — Returns how many `ArtifactCard`s are currently in the player's hand.

### FuseRuntime

- `void FuseRuntime.Mark(CardModel card)`
  — Flags a card as currently mid-fuse (used by `FusePatches` to spend reduced resources).
- `bool FuseRuntime.TryConsume(CardModel card)`
  — Clears the flag and returns whether it was set.
- `bool FuseRuntime.IsActive(CardModel card)`
  — Checks the flag without clearing.

### Files

| File | Role |
|---|---|
| `Cards/Artifacts/ArtifactCard.cs` | Base class; fuse-or-raw play flow |
| `Cards/Artifacts/ArtifactTier.cs` | T0/T1/T2/T3 enum |
| `Cards/Artifacts/ArtifactTag.cs`, `ArtifactType.cs` | Custom CardTag / CardType enums |
| `Cards/Artifacts/FuseRecipes.cs` | Recipe lookup + valid-discard logic |
| `Cards/Artifacts/FuseKeyword.cs` | `Fuse` custom `CardKeyword` |
| `Cards/Artifacts/IFuseListener.cs` | Reactor interface |
| `Cards/Keywords/IFuseCard.cs` | Marker interface declaring `FuseCost` |
| `Cards/Keywords/FuseRuntime.cs` | Transient fuse-in-progress flag |
| `Patches/FusePatches.cs` | Energy-refund Harmony patches |

---

## Other custom keywords

### Countdown

- `interface ICountdownCard`
  — Exposes `int CountdownRemaining { get; set; }` that ticks down each turn.
- `Task CountdownHelper.Tick(PlayerChoiceContext ctx, CardModel card)`
  — Decrements `CountdownRemaining`; exhausts the card when it hits 0.

Dispatched from `KeywordDispatcherPower.AfterPlayerTurnStart` across Hand/Draw/Discard.

### Crystallize

- `interface ICrystallizeCard`
  — Exposes `int CrystallizeCost { get; }` (reduced play cost) and `Task OnCrystallize(ctx)`.
- `void CrystallizeRuntime.Mark(CardModel card)`
  — Flags a card as currently being played via its Crystallize path.
- `bool CrystallizeRuntime.TryConsume(CardModel card)` / `IsActive(CardModel card)`
  — Clear-and-return / peek the flag.

Play-path patched by `Patches/CrystallizePlayPatch.cs`.

### Last Words

- `interface ILastWordsCard`
  — `Task OnLastWords(PlayerChoiceContext ctx)` — Fires on the card's exhaust.
- `interface ILastWordsEnchantment`
  — `Task OnLastWords(PlayerChoiceContext ctx, CardModel card)` — Fires on host-card exhaust.

Dispatched from `KeywordDispatcherPower.AfterCardExhausted`.

### On Turn Start

- `interface IOnTurnStartCard`
  — `Task OnTurnStart(PlayerChoiceContext ctx)` — Fires at the start of the player's turn.

Dispatched from `KeywordDispatcherPower.AfterPlayerTurnStart`, before countdown ticks.

### Skybound Art / Super Skybound Art (placeholders)

- `CardKeyword SkyboundArtKeyword.SkyboundArt`
  — Placeholder keyword (tooltip only; gauge ≥ 10 ability not yet implemented).
- `CardKeyword SuperSkyboundArtKeyword.SuperSkyboundArt`
  — Placeholder keyword (tooltip only; gauge ≥ 15 variant not yet implemented).

---

## Custom CardKeywords / CardTags / CardTypes

| Name | Kind | File |
|---|---|---|
| `FuseKeyword.Fuse` | `CardKeyword` | `Cards/Artifacts/FuseKeyword.cs` |
| `CountdownKeyword.Countdown` | `CardKeyword` | `Cards/Keywords/CountdownKeyword.cs` |
| `CrystallizeKeyword.Crystallize` | `CardKeyword` | `Cards/Keywords/CrystallizeKeyword.cs` |
| `LastWordsKeyword.LastWords` | `CardKeyword` | `Cards/Keywords/LastWordsKeyword.cs` |
| `SkyboundArtKeyword.SkyboundArt` | `CardKeyword` | `Cards/Keywords/SkyboundArtKeyword.cs` |
| `SuperSkyboundArtKeyword.SuperSkyboundArt` | `CardKeyword` | `Cards/Keywords/SuperSkyboundArtKeyword.cs` |
| `ArtifactTag.Artifact` | `CardTag` | `Cards/Artifacts/ArtifactTag.cs` |
| `PuppetTag.Puppet` | `CardTag` | `Cards/Puppets/PuppetTag.cs` |
| `OmenTag.Omen` | `CardTag` | `Cards/Omen/OmenTag.cs` |
| `ArtifactType.Artifact` | `CardType` | `Cards/Artifacts/ArtifactType.cs` |

---

## Harmony patches (behavior modifiers)

| Patch | Target | Purpose |
|---|---|---|
| `FusePatches.FuseAffordabilityPatch` | `PlayerCombatState.HasEnoughResourcesFor` | Makes an Artifact with a valid fusion partner "playable" with less energy than its raw cost. |
| `FusePatches.FuseSpendResourcesPatch` | `CardModel.SpendResources` | Spends `FuseCost` instead of the full energy cost when the fuse path is taken. |
| `CrystallizePlayPatch` | (crystallize play flow) | Dispatches `ICrystallizeCard.OnCrystallize` on the reduced-cost play path. |
| `ArtifactCardFramePatch` | (card frame rendering) | Applies the Artifact visual frame. |
| `CardPlayAudioPatch` | Card-play flow | Triggers the mod's custom FMOD voice lines when a card is played. |
| `EvoHolderInjectPatch` | `NCombatRoom._Ready` | Adds the EP and SEP holders next to the energy icon (Portalcraft only). |
| `NCardEvoTargetingPatch.OnFocusPatch` | `NHandCardHolder.OnFocus` | Forwards card hover to `NTargetManager` while evo targeting is active. |
| `NCardEvoTargetingPatch.OnUnfocusPatch` | `NHandCardHolder.OnUnfocus` | Forwards card un-hover while evo targeting is active. |
| `NCardEvoTargetingPatch.StartCardPlayPatch` | `NPlayerHand.StartCardPlay` | Suppresses card-play on release while evo targeting is active. |
| `NCardEvoTargetingPatch.CreateHoverTipsPatch` | `NCardHolder.CreateHoverTips` | Suppresses card tooltips during evo targeting (avoids NRE in `NHoverTipSet.SetFollowOwner`). |

---

## Base classes

- `abstract class sts2_char_portalcraftCard : CustomCardModel`
  — Card base; auto image-paths, gold glow for evolved cards, pool registration.
- `abstract class sts2_char_portalcraftRelic : CustomRelicModel`
  — Relic base.
- `abstract class sts2_char_portalcraftPotion : CustomPotionModel`
  — Potion base.
- `abstract class sts2_char_portalcraftPower : CustomPowerModel`
  — Power base (auto power-icon path resolution).
- `abstract class ArtifactCard : sts2_char_portalcraftCard, IFuseCard`
  — Base for the Artifact line; handles fuse-or-raw-play + keywords automatically.
- `class PortalcraftConfig : SimpleModConfig`
  — Mod settings (e.g. `CardSfxVolume`).

Base-card override of note:

- `protected override bool ShouldGlowGoldInternal => EvoRuntime.GetTier(this) != null`
  — Declares that any Portalcraft card glows gold while evolved.

---

## KeywordDispatcherPower — central dispatcher

- `int ArtifactsExhaustedCount { get; }`
  — Running count of `ArtifactCard`s exhausted this combat (for relics/powers that scale with it).
- `Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)`
  — Resets `EvoRuntime` turn lockout, fires `IOnTurnStartCard.OnTurnStart`, ticks `ICountdownCard`s.
- `Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)`
  — Bumps `ArtifactsExhaustedCount`, fires `ILastWordsCard.OnLastWords` and `ILastWordsEnchantment.OnLastWords`.
- `decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)`
  — Adds `EvolveDamageBonus` / `SuperEvolveDamageBonus` when `cardSource` is evolved/super-evolved.
- `decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)`
  — Adds `EvolveBlockBonus` / `SuperEvolveBlockBonus` when the block source is an evolved card.

---

## Project layout

```
sts2_char_portalcraftCode/
├── Cards/
│   ├── Artifacts/          # Artifact + fuse infrastructure
│   ├── Keywords/           # Tag interfaces + runtime state bags + EvoCmd
│   ├── Omen/               # Amulet tokens
│   ├── Puppets/            # Puppet tokens
│   └── sts2_char_portalcraftCard.cs
├── Character/              # Character definition + pool registrations
├── Extensions/             # Asset-path helpers
├── Patches/                # Harmony patches
├── Powers/                 # Power implementations
├── Relics/                 # Relic implementations
├── UI/                     # Custom UI nodes (NEvoHolder)
├── Audio/                  # CardPlayAudioManager
└── MainFile.cs             # ModInitializer entry point

sts2_char_portalcraft/      # Godot resource dir
├── audio/                  # voice lines
├── images/                 # Card portraits, charui, powers, relics
├── localization/{eng,jpn,zhs}
└── scenes/                 # Character scenes
```
