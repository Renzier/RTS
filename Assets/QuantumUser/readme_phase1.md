# Anachron Phase 1 Checkpoint

## Scope Completed

- Local deterministic Quantum session starts from `QuantumGameScene`.
- RTS camera supports angled perspective, mouse wheel zoom, and WASD/arrow panning.
- Player-owned workers can be selected and commanded.
- Selected workers path on the baked Quantum navmesh.
- Resource nodes and main bases spawn from deterministic bootstrap data.
- Workers gather resources over time, return to base when full, deposit, and resume gathering.
- Player economy tracks wood, iron, food usage, and defeated state.
- Main base health/status shell exists for all three prototype players.
- Prototype HUD shows player state, base health, worker state, carry amounts, and resource depletion.

## Phase 1 Scene Components

`AnachronQuantumInput` owns the active prototype view layer:

- `AnachronQuantumInput`
- `AnachronSelectionDebugOverlay`
- `AnachronSelectablePrimitiveView`
- `AnachronPrototypeHud`

Older debug view scripts remain available on disk for reference, but are no longer attached to the scene.

## Phase 2 Entry Notes

- Bootstrap-spawned test entities now read from `AnachronPrototypeScenario`.
- Replace `AnachronPrototypeScenario` with explicit prototype/config assets when the data shape stabilizes.
- Replace primitive cylinders with proper unit/building/resource view prefabs.
- Add command/state components only through Quantum schema changes and CodeGen.
- Keep all authoritative gameplay in `Assets/QuantumUser/Simulation`.
- Keep Unity APIs out of the Simulation assembly.

## Phase 2 First Target

Add deterministic tech progression state:

- Player tech tier state added through `PlayerTechState`.
- Instant resource deduction for upgrades added through `TechUpgradeSystem`.
- Upgrade command input path uses `T` key and `UpgradeIntent`.
- HUD visibility shows current tier and last upgrade result.

Run Quantum CodeGen after this schema change before testing in Unity.

## Phase 2 Hero Shell

- `PlayerHeroState` tracks one hero slot per player.
- `HeroLifecycleSystem` keeps hero status tied to main base and defeat state.
- Prototype HUD displays hero active/rebuild/inactive state.
- Combat, hero entity views, hero death commands, and rebuild commands are not implemented yet.

Run Quantum CodeGen after this schema change before testing in Unity.
