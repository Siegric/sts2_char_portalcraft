// Bane damage is handled entirely by KeywordDispatcherPower.ModifyDamageAdditive:
//   - vs non-minion: +10 flat damage
//   - vs minion:     +9999 damage (lethal, flows through normal death hooks)
// No Harmony patch needed — the damage modification pipeline already exposes
// per-target hooks, which is cleaner than patching CreatureCmd.Damage.
