# Faction ID Expansion Strategy

Purpose: decide how this repo should support all eight Kilnfall powers without destabilizing the current three-faction Photon Quantum prototype.

## Decision

Extend the current integer `FactionId` constants into canonical Kilnfall IDs, while keeping the existing `Tech`, `Fantasy`, and `Hybrid` constants as compatibility aliases during the transition.

Recommended ID table:

- `ArdentConcord = 0`
- `Wrought = 1`
- `Gharn = 2`
- `Seethe = 3`
- `Veirn = 4`
- `Vaelun = 5`
- `Nimhara = 6`
- `Virii = 7`

Compatibility aliases:

- `Tech = ArdentConcord`
- `Fantasy = Wrought`
- `Hybrid = Gharn`

Do not renumber existing saved/player assumptions. Player 0 should continue to start as Ardent Concord, player 1 as Wrought, and player 2 as Gharn until an explicit scenario-selection sprint changes that.

## Rationale

The current code stores faction IDs as deterministic `Int32` values in Quantum state, not as a generated enum. That makes extension low-risk as long as the first three values remain stable.

Replacing the old IDs everywhere in one broad rename would create unnecessary churn across simulation, view, generated files, and handoff docs. Keeping aliases lets new work use Kilnfall names while older systems continue to compile until they are touched naturally.

Keeping only the prototype IDs and mapping display names forever would make future faction-specific logic harder to read, especially for Seethe through Virii. The next sprints need IDs that mean the same thing in simulation, stats, display helpers, and scenario setup.

## Implementation Rules

- Add canonical constants to `Assets/QuantumUser/Simulation/FactionId.cs` before adding the fourth faction.
- Keep `Tech`, `Fantasy`, and `Hybrid` as aliases until no active code references them.
- Update `Normalize` to accept all valid Kilnfall IDs and default unknown values to `ArdentConcord`.
- Prefer `FactionId.ArdentConcord`, `FactionId.Wrought`, and `FactionId.Gharn` in new code.
- Do not change `.qtn` schema for this strategy. Existing `Int32 FactionId` state is sufficient.
- Do not add faction mechanics while adding IDs. First expansion sprints should add one faction at a time with display names, colors, basic stats, and bootstrap assignability only.
- Treat Virii as assignable only in Sprint 39 or later. Virii copying still waits on the Grain/Cast/Seal contract work.

## Sprint Order Impact

Sprint 35 should add Seethe as ID `3` without changing existing Ardent Concord, Wrought, or Gharn behavior.

After each new faction ID sprint, smoke-test:

- Existing P0/P1/P2 starts still map to Ardent Concord, Wrought, and Gharn.
- The new faction can be assigned in the bootstrap scenario or local start selector.
- HUD/debug labels show the canonical Kilnfall name.
- Unknown/out-of-range IDs still normalize to Ardent Concord.
