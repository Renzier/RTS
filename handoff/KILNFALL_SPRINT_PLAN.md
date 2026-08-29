# Kilnfall Sprint Plan

Created: 2026-08-23.

This plan breaks the Kilnfall conversion into small, low-risk sprints. Each sprint should be completed, compiled, and manually smoke-tested before starting the next. Avoid combining sprints unless the user explicitly asks for a larger batch.

First Hand update, 2026-08-23: authenticated internal source review confirmed that the current small-sprint approach is still correct, but later work needs explicit gates for Photon Quantum adaptation of MovementDomain, Grain/Cast/Seal data contracts, and Ardent Concord as the pathfinder faction. Keep Sprints 1-8 as low-risk presentation conversion.

## Sprint Rules

- One sprint equals one feature or one contained change.
- Keep deterministic gameplay decisions in `Assets/QuantumUser/Simulation`.
- Keep Unity view scripts presentation-only, except for input submission.
- Prefer display-name and view-only changes before `.qtn` schema changes.
- If a sprint changes `.qtn` files, run Quantum CodeGen before expecting Unity compile results to be meaningful.
- Update `handoff/PROJECT_STATE.md` after each completed sprint.
- Update this file by marking sprint status if a sprint is finished, deferred, or replaced.

Status key:

- `[ ]` Not started
- `[~]` In progress
- `[x]` Complete
- `[!]` Blocked or needs decision

## Foundation Conversion

### Sprint 1: Rename Prototype Faction Display Names

Status: `[x]`

Implementation note: display-name changes were applied to the HUD and player-state debug overlay. Unity batchmode compile verification was attempted on 2026-08-23 but blocked before script compilation by Unity licensing initialization failure. Manual Unity verification was later provided by screenshot: the in-game HUD shows Ardent Concord, Wrought, and Gharn.

Goal: Change visible faction names only.

Scope:

- Tech displays as Ardent Concord.
- Fantasy displays as Wrought.
- Hybrid displays as Gharn.
- Do not rename enum IDs or schema fields yet.

Likely files:

- `Assets/QuantumUser/Simulation/FactionId.cs`
- `Assets/QuantumUser/View/AnachronPlayerStateDebugOverlay.cs`
- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`

Acceptance:

- HUD and debug overlay show Ardent Concord, Wrought, and Gharn.
- Player assignment remains P0/P1/P2 with no gameplay behavior change.
- Project compiles.

### Sprint 2: Rename Unit Display Labels

Status: `[x]`

Implementation note: HUD worker/hero display labels were updated without changing unit kinds, stats, schema, or gameplay behavior.

Goal: Replace generic worker/hero labels with first-pass Kilnfall labels.

Scope:

- Ardent Concord worker: Keelwatch Ranker or Dockhand.
- Wrought worker: Wright.
- Gharn worker: Sinterjack.
- Ardent Concord hero: Concord Marshal.
- Wrought hero: Wrought Overseer.
- Gharn hero: Tally Captain.
- Do not change unit kinds or stats.

Likely files:

- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`
- `Assets/QuantumUser/View/AnachronSelectablePrimitiveView.cs`

Acceptance:

- Selection panel shows faction-specific unit labels.
- Existing select/move/gather/attack behavior still works.

### Sprint 3: Rename Main Building Labels

Status: `[x]`

Implementation note: HUD and player-state debug overlay main building labels were updated to Ledger House, Longhold Node, and Oathpyre without changing building behavior, HP, tiers, or defeat logic.

Goal: Replace generic main base labels with faction-specific names.

Scope:

- Ardent Concord main: Ledger House.
- Wrought main: Longhold Node.
- Gharn main: Oathpyre.
- Behavior remains identical.

Likely files:

- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`
- `Assets/QuantumUser/View/AnachronSelectablePrimitiveView.cs`

Acceptance:

- Selection panel and world labels show faction-specific main building names.
- Main-base defeat still triggers correctly.

### Sprint 4: Rename Supply Building Labels

Status: `[x]`

Implementation note: HUD supply/foundation/deconstruction/build command labels were updated to Countersign Post, Count Relay, and Tally Stone without changing construction, cancellation, refund, placement, or food-cap behavior.

Goal: Replace generic supply building labels with faction-specific names.

Scope:

- Ardent Concord supply: Countersign Post.
- Wrought supply: Count Relay.
- Gharn supply: Tally Stone.
- Keep current construction, cancellation, refund, and food-cap behavior.

Likely files:

- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`
- `Assets/QuantumUser/View/AnachronSelectablePrimitiveView.cs`
- `Assets/QuantumUser/View/AnachronBuildPlacementPreview.cs`

Acceptance:

- Build preview, construction HUD, and selected building label use faction-specific names.
- `C` build and `X` cancel/deconstruct still work.

### Sprint 5: Rename Resource Display Language

Status: `[x]`

Implementation note: HUD and debug overlay resource labels were updated to Salvage, Plate, and Holding without changing `ResourceKind`, economy math, costs, gathering, deposits, or capacity behavior.

Goal: Reframe resources without altering economy math.

Scope:

- Wood displays as Salvage.
- Iron displays as Plate.
- Food cap displays as Holding.
- Keep underlying resource IDs unchanged.

Likely files:

- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`
- `Assets/QuantumUser/View/AnachronEconomyPrimitiveView.cs`
- `Assets/QuantumUser/Simulation/ResourceKind.cs` only if display helpers live there.

Acceptance:

- HUD resource labels use Kilnfall language.
- Gathering/depositing values are unchanged.

## Visual Identity

### Sprint 6: Update Faction Colors

Status: `[x]`

Implementation note: primitive view colors were shifted toward the first three Kilnfall palettes: Ardent Concord grey-blue/brass-orange, Wrought dark industrial yellow, and Gharn furnace red/iron. Selection, health, and placement validity colors were left unchanged.

Goal: Make the three prototype factions visually read as Kilnfall powers.

Scope:

- Ardent Concord: grey-blue metal, brass, lifeboat orange accents.
- Wrought: dark machine plate, dull industrial yellow/white lights.
- Gharn: ash black, furnace red, raw iron.
- View-only color changes.

Likely files:

- `Assets/QuantumUser/View/AnachronSelectablePrimitiveView.cs`
- `Assets/QuantumUser/View/AnachronPlayerStateDebugOverlay.cs`
- `Assets/QuantumUser/View/AnachronBuildPlacementPreview.cs`

Acceptance:

- Factions are easy to distinguish.
- Selection, health, and placement valid/invalid colors remain readable.

### Sprint 7: Update Primitive Silhouettes

Status: `[x]`

Implementation note: existing primitive types and scale profiles were tuned so Ardent Concord reads compact and standardized, Wrought reads blocky/machine-like, and Gharn reads heavier and grounded. No new gameplay logic, schema, or selection code was added.

Goal: Improve existing primitive shapes to better match the three factions.

Scope:

- Ardent Concord: practical, standardized, compact human/shipyard shapes.
- Wrought: blocky machine forms and modular plates.
- Gharn: heavier grounded silhouettes and furnace/oath markers.
- No new gameplay logic.

Likely files:

- `Assets/QuantumUser/View/AnachronSelectablePrimitiveView.cs`

Acceptance:

- Worker, hero, main, and supply silhouettes differ by faction.
- No selection collider or clickability regression.

### Sprint 8: Update Construction Visual States

Status: `[x]`

Goal: Make foundations and deconstruction read as Kilnfall construction.

Scope:

- Ardent Concord foundations look like signed scaffolds or tally posts.
- Wrought foundations look like machine-plate assembly.
- Gharn foundations look like ash/furnace/tally stones.
- View-only.

Likely files:

- `Assets/QuantumUser/View/AnachronSelectablePrimitiveView.cs`
- `Assets/QuantumUser/View/AnachronBuildPlacementPreview.cs`

Acceptance:

- In-progress buildings visually differ from complete buildings.
- Progress display still works.

Implementation note:

- Sprint 8 updated view-only support-building states.
- Ardent Concord, Wrought, and Gharn foundations now use distinct construction colors.
- Completed support buildings use a separate deconstruction color while deconstructing.
- Support buildings lower during deconstruction, while construction remains a low foundation shape.
- The placement preview was flattened slightly so it reads more like a foundation footprint.
- No Quantum schema, construction timing, refund, health, placement, or selection logic was changed.

## Map And Environment

### Sprint 9: Rename Prototype Scenario Concept

Status: `[x]`

Goal: Reframe the current map as a Kilnfall location in code comments/HUD-facing labels.

Scope:

- Name the current ground map something like Quill Waist Shard or Ashenspar Quill-Waist.
- No geometry changes.

Likely files:

- `Assets/QuantumUser/Simulation/AnachronPrototypeScenario.cs`
- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`

Acceptance:

- Any visible map/scenario label uses the new Kilnfall name.
- No spawn/resource placement changes.

Implementation note:

- Added `AnachronPrototypeScenario.ScenarioName` as `Ashenspar Quill-Waist`.
- Updated the HUD map row to show the Kilnfall scenario name while preserving resource/base/support counts.
- No spawn positions, resource amounts, map bounds, navmesh, schema, or gameplay behavior was changed.

### Sprint 10: Add View-Only Environmental Landmarks

Status: `[x]`

Goal: Add non-interactive visual markers for the Kilnfall world.

Scope:

- Add a central/distant Quill waist marker or tower silhouette.
- Add water/abyss boundary cues if feasible in the existing scene.
- No gameplay collision or pathing change.

Likely files:

- `Assets/QuantumUser/Scenes/QuantumGameScene.unity`
- Possibly a new view script under `Assets/QuantumUser/View/`

Acceptance:

- Scene visually suggests a Quill/Shards/Mere battlefield.
- Movement and placement validation are unchanged.

Implementation note:

- Added runtime-only Ashenspar view landmarks in `AnachronSelectablePrimitiveView`.
- Landmarks include Mere boundary bands, a distant Quill waist spire/ring, and shard ridge markers.
- Landmark colliders are disabled and removed so they do not affect Unity-side interaction, Quantum simulation, navmesh, pathing, placement validation, or combat.
- No scene asset, spawn/resource layout, map bounds, navmesh, schema, or gameplay behavior was changed.

### Sprint 11: Reposition Resource Nodes Around Kilnfall Landmarks

Status: `[x]`

Goal: Make the existing resource layout feel like salvage and industrial deposits.

Scope:

- Adjust current six resource node positions only if needed.
- Preserve fair access for all three players.
- No new resource types.

Likely files:

- `Assets/QuantumUser/Simulation/AnachronPrototypeScenario.cs`

Acceptance:

- All players can reach resources.
- Workers gather/deposit correctly.
- No nodes overlap bases, supplies, or blocked terrain.

Implementation note:

- Moved only the upper resource pair to sit nearer the shard ridge landmarks.
- Wood node moved from `(-10, 10)` to `(-14, 18)`.
- Iron node moved from `(10, 10)` to `(14, 18)`.
- The other four resource nodes, all resource amounts, resource types, bases, units, map bounds, navmesh, schema, and gameplay rules were unchanged.

### Sprint 12: Add Central Quill-Waist Objective Landmark

Status: `[x]`

Goal: Add a neutral central structure as a visible strategic target, without capture logic yet.

Scope:

- Deterministic entity or view-only marker depending what is safest after inspection.
- No victory or resource effect.

Likely files:

- `Assets/QuantumUser/Simulation/AnachronUnits.qtn` only if entity/component support is needed.
- `Assets/QuantumUser/Simulation/TestSelectableBootstrapSystem.cs`
- `Assets/QuantumUser/View/AnachronSelectablePrimitiveView.cs`

Acceptance:

- Landmark exists and is visually distinct.
- It does not interfere with existing combat/build placement unless intentionally blocked.
- If schema changes occur, CodeGen has been run.

Implementation note:

- Added a view-only `CentralQuillWaistObjective` cylinder and `CentralQuillWaistRing` to the Ashenspar runtime landmarks.
- The marker is non-colliding and has no Quantum entity, capture state, score, resource effect, targetable component, pathing effect, or placement effect.
- No schema changes were made, so Quantum CodeGen was not required.

## Construction And Pathing Stability

### Sprint 13: Dynamic NavMesh Obstacle For Completed Supply Buildings

Status: `[x]`

Goal: Make completed placed buildings affect navigation.

Scope:

- Only completed supply buildings become obstacles.
- Foundations can remain non-blocking unless existing placement rules require otherwise.

Likely files:

- `Assets/QuantumUser/Simulation/SupplyBuildingConstructionSystem.cs`
- `Assets/QuantumUser/Simulation/NavMeshMovementRequestSystem.cs`
- `Assets/QuantumUser/Editor/AnachronPhase1NavMeshSetup.cs`
- Scene/NavMesh assets as needed.

Acceptance:

- Units route around completed supply buildings.
- Existing build placement still prevents overlap.
- Deconstructed buildings stop blocking when removed.

Implementation note:

- Completed, healthy support buildings now receive a `NavMeshAvoidanceObstacle` with radius `1.35`.
- Foundations, deconstructing buildings, canceled foundations, destroyed buildings, and removed buildings do not keep the obstacle.
- Existing placement validation remains unchanged.
- No schema or CodeGen changes were required.
- Unity verification showed the avoidance obstacle alone did not stop units from moving through completed buildings.
- Added `SupplyBuildingCollisionSystem` as a deterministic fallback: live units that enter a completed support building footprint are pushed back outside its blocking radius.
- Follow-up verification showed units now move around buildings, but the correction felt like scraping/dragging along a wall.
- Added a target-aware slide bias to `SupplyBuildingCollisionSystem` so correction pushes units outward and slightly around the building toward their current nav target.
- `NavMeshMovementRequestSystem` also nudges clicked move targets outside completed support building footprints if the player clicks inside one.
- Video review showed units could jump quickly during correction, so collision correction is now capped at `0.12` units per tick.
- Runtime pathing still needs another Unity smoke test to confirm the smoother fallback feels acceptable.

### Sprint 14: Worker Build Mode Before Placement Preview

Status: `[x]`

Goal: Stop showing the building placement shadow just because a worker is selected.

Scope:

- Selecting a worker should not automatically show the support building preview.
- Pressing `B` should enter/open build mode for the selected worker.
- Build mode should let the player choose what building to place.
- The first implementation can expose only the existing support building option.
- Preserve deterministic simulation validation for the final placement.
- Keep existing construction, cancellation, refund, and deconstruction rules.

Likely files:

- `Assets/QuantumUser/View/AnachronQuantumInput.cs`
- `Assets/QuantumUser/View/AnachronBuildPlacementPreview.cs`
- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`
- `Assets/QuantumUser/Simulation/SupplyBuildingConstructionSystem.cs` only if the final submitted intent changes.

Acceptance:

- Worker selection alone does not show a placement ghost.
- Pressing `B` enters/opens build mode.
- Player can choose the existing support building from build mode.
- Placement preview appears only while build mode/placement mode is active.
- Existing support building construction still works after selection.

Implementation note:

- Added view-side `AnachronQuantumInput.BuildModeActive`.
- Pressing `B` with a selected owned worker enters build mode instead of immediately placing or showing the ghost.
- Pressing `B` without a selected owned worker keeps the existing train-worker input path.
- Pressing `C` while build mode is active submits the existing support-building placement intent and exits build mode.
- `Escape` or losing worker selection exits build mode.
- `AnachronBuildPlacementPreview` only shows the placement ghost while build mode is active.
- Worker HUD now shows `Build: press B`, then the existing support building option while build mode is active.
- No Quantum schema, placement validation, construction, cancellation, refund, or deconstruction logic changed.

### Sprint 14B: Build Placement Snap Or Grid Preview

Status: `[x]`

Goal: Make supply placement easier and less error-prone.

Scope:

- View/input side only unless deterministic snapped coordinates must be submitted.
- Keep current validation rules.

Likely files:

- `Assets/QuantumUser/View/AnachronBuildPlacementPreview.cs`
- `Assets/QuantumUser/View/AnachronQuantumInput.cs`
- `Assets/QuantumUser/Simulation/SupplyBuildingConstructionSystem.cs` only if simulation snapping is required.

Acceptance:

- Preview position is stable and clear.
- Server/simulation validation still decides final legality.

Implementation note:

- Added a view-only local grid around the support-building placement preview.
- The grid appears only while worker build mode is active.
- Placement still uses the exact cursor world position; no snapping was added.
- Grid lines have disabled/removed colliders and do not affect selection, placement, pathing, construction, or Quantum simulation.
- No schema, input intent, validation, construction, cancellation, refund, or deconstruction logic changed.

### Sprint 15: Improve Placement Failure Feedback

Status: `[x]`

Goal: Explain why building placement is invalid.

Scope:

- Add one concise HUD/status reason: out of bounds, too close to resource, too close to unit/building, insufficient resources.
- No behavior change.

Likely files:

- `Assets/QuantumUser/Simulation/SupplyBuildingConstructionSystem.cs`
- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`
- `.qtn` only if current result state cannot carry a reason.

Acceptance:

- Invalid placement gives a useful reason.
- Existing valid/invalid ghost remains intact.

Implementation note:

- `AnachronBuildPlacementPreview` now exposes a view-only `PlacementStatus` string while build mode is active.
- Worker HUD shows the current placement status under the support-building build option.
- Reasons include insufficient Salvage/Plate, too far from builder, outside build area, too close to resource, too close to building, too close to support, and too close to unit.
- Valid placement shows `Valid placement`.
- Existing valid/invalid ghost color remains intact.
- No Quantum schema, input intent, simulation validation, placement, construction, cancellation, refund, or deconstruction logic changed.

## First Three Faction Mechanics

### Sprint 16: Ardent Concord Repair Identity

Status: `[x]`

Goal: Give Ardent Concord a small repair/sustain advantage.

Scope:

- Pick one narrow effect: faster completed supply repair later, cheaper repair, or passive out-of-combat building repair.
- Do not implement Halving/Fastening yet.

Likely files:

- `Assets/QuantumUser/Simulation/FactionStats.cs`
- Relevant repair system if repair exists; otherwise defer until repair sprint.

Acceptance:

- Concord has one visible sustain advantage.
- Other factions are unchanged.

Implementation note:

- Added `ArdentConcordRepairSystem`.
- Ardent Concord owned main buildings and completed support buildings mend `5` HP every `60` simulation ticks while damaged and alive.
- Foundations and deconstructing support buildings do not mend.
- Wrought and Gharn buildings are unchanged.
- The system syncs repaired building health back to `Targetable` so combat, HUD, and selection health stay consistent.
- Follow-up HUD readability pass: selected main and support buildings now show health as a labeled bar with larger centered HP numbers, and world-space health bars now receive screen-space HP number labels.
- Added prototype debug damage: select an owned building and press `V` to deal `250` test damage without destroying it, making passive repair easy to verify.
- No Quantum schema, CodeGen, construction, faction assignment, final combat damage rules, or normal player attack behavior changed.

### Sprint 17: Wrought Building Durability Identity

Status: `[x]`

Goal: Give Wrought a contained building durability advantage.

Scope:

- Higher supply/main building HP or foundation HP.
- No new network mechanic yet.

Likely files:

- `Assets/QuantumUser/Simulation/FactionStats.cs`
- `Assets/QuantumUser/Simulation/SupplyBuildingConstructionSystem.cs`
- `Assets/QuantumUser/Simulation/TestSelectableBootstrapSystem.cs`

Acceptance:

- Wrought buildings have intended HP values.
- Combat/destruction cleanup still works.

Implementation note:

- Wrought Longhold Node max HP increased to `1800`.
- Wrought Count Relay max HP increased to `650`.
- Ardent Concord and Gharn support buildings remain `500` max HP.
- Support-building max health now comes from `FactionStats`.
- No Quantum schema, CodeGen, construction timing, costs, refunds, repair rules, combat damage, or destruction cleanup changed.

### Sprint 18: Gharn Hold-Ground Combat Identity

Status: `[x]`

Goal: Give Gharn a simple first-pass ground identity.

Scope:

- Add a small combat bonus while not moving or while holding position.
- Keep it limited to units, not buildings.
- Avoid oath/tally permanence for now.

Likely files:

- `Assets/QuantumUser/Simulation/AttackDamageSystem.cs`
- `Assets/QuantumUser/Simulation/FactionStats.cs`
- `Assets/QuantumUser/Simulation/MovementMode.cs`

Acceptance:

- Gharn units get the bonus only under the chosen condition.
- Moving cancels or prevents the bonus.
- Combat remains deterministic.

Implementation note:

- Added a first-pass Gharn hold-ground damage bonus through `FactionStats.HoldGroundDamageBonus`.
- Gharn heroes gain `+8` attack damage only when they have no active move target.
- Chasing into range does not receive the bonus; once `AttackTargetingSystem` clears movement because the hero is in range, subsequent attacks receive the bonus.
- Ardent Concord and Wrought hold-ground bonus values are `0`.
- The effect is limited to combat units, currently heroes, and does not apply to buildings.
- No Quantum schema, CodeGen, movement mode, target acquisition, cooldown, or base hero damage values changed.

## Production Depth

### Sprint 19: Add Faction-Specific Worker Costs

Status: `[x]`

Goal: Let each initial faction have slightly different worker economy tuning.

Scope:

- Ardent Concord: balanced.
- Wrought: slightly more expensive/tougher.
- Gharn: cheaper or faster but less economic efficiency, if desired.
- One tuning pass only.

Likely files:

- `Assets/QuantumUser/Simulation/FactionStats.cs`
- `Assets/QuantumUser/Simulation/WorkerProductionSystem.cs`

Acceptance:

- Worker production costs are faction-aware.
- HUD affordability feedback still works.

Implementation note:

- Added worker production costs to `FactionStats`.
- Ardent Concord Keelwatch Ranker remains `50` Salvage / `25` Plate / `1` Holding.
- Wrought Wright now costs `65` Salvage / `35` Plate / `1` Holding.
- Gharn Sinterjack now costs `40` Salvage / `20` Plate / `1` Holding.
- `WorkerProductionSystem` now checks and deducts faction-specific worker costs.
- Main-building HUD worker-production labels now read the same faction-specific costs and shortfall values.
- Existing worker max health, spawn placement, food-cap behavior, selection, movement, and gathering behavior were unchanged.
- No Quantum schema or CodeGen changes were required.

### Sprint 20: Add Faction-Specific Supply Costs

Status: `[x]`

Goal: Let Countersign Post, Count Relay, and Tally Stone have distinct costs.

Scope:

- Cost only, no new effects.

Likely files:

- `Assets/QuantumUser/Simulation/FactionStats.cs`
- `Assets/QuantumUser/Simulation/SupplyBuildingConstructionSystem.cs`
- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`

Acceptance:

- Build command charges the correct faction-specific cost.
- Refund/deconstruction uses the original paid cost correctly.

Implementation note:

- Added support-building costs to `FactionStats`.
- Ardent Concord Countersign Post remains `100` Salvage / `50` Plate.
- Wrought Count Relay now costs `120` Salvage / `70` Plate.
- Gharn Tally Stone now costs `90` Salvage / `45` Plate.
- `SupplyBuildingConstructionSystem` now checks, deducts, and stores faction-specific support-building costs.
- Construction cancel and completed deconstruction refunds continue to use the stored original paid cost.
- Worker build-mode HUD and placement failure feedback now read faction-specific support-building affordability.
- Fixed placement preview affordability to check the active local player slot instead of always checking player `0`.
- Existing construction timing, food provided, health, placement geometry validation, builder assignment, and deconstruction timing were unchanged.
- No Quantum schema or CodeGen changes were required.

### Sprint 21: Add Faction-Specific Supply Cap Values

Status: `[ ]`

Goal: Let supply buildings grant different capacity by faction.

Scope:

- Ardent Concord: standard.
- Wrought: lower or higher based on balance direction.
- Gharn: standard or high to support ground pressure.
- No other building behavior changes.

Likely files:

- `Assets/QuantumUser/Simulation/FactionStats.cs`
- `Assets/QuantumUser/Simulation/SupplyBuildingConstructionSystem.cs`

Acceptance:

- Completed supply grants correct faction-specific cap.
- Deconstruction removes the correct amount.

### Sprint 22: Add Multi-Worker Construction Speed

Status: `[ ]`

Goal: Allow multiple workers to speed construction, if this remains desired.

Scope:

- Implement generic multi-worker construction first.
- No faction bonuses in this sprint.

Likely files:

- `Assets/QuantumUser/Simulation/AnachronUnits.qtn`
- `Assets/QuantumUser/Simulation/SupplyBuildingConstructionSystem.cs`
- `Assets/QuantumUser/Simulation/MoveCommandIntentSystem.cs`

Acceptance:

- More assigned builders reduce construction time deterministically.
- Builder assignment/cleanup handles death, cancel, and completion.
- CodeGen completed if schema changes.

### Sprint 23: Add Repair Command

Status: `[ ]`

Goal: Add basic worker repair for damaged friendly buildings.

Scope:

- Generic repair only.
- No Concord-specific mechanic yet unless Sprint 16 was deferred to here.

Likely files:

- `Assets/QuantumUser/Simulation/AnachronInput.qtn`
- `Assets/QuantumUser/Simulation/MoveCommandIntentSystem.cs`
- New or existing repair system under `Assets/QuantumUser/Simulation/`
- `Assets/QuantumUser/View/AnachronQuantumInput.cs`
- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`

Acceptance:

- Workers can repair friendly damaged buildings.
- Repair costs/resources are deterministic.
- Combat damage and death cleanup still work.

## Quill Objective Path

### Sprint 24: Selectable Quill Landmark

Status: `[ ]`

Goal: Make the central Quill landmark selectable.

Scope:

- Selection label and health/neutral info only.
- No capture logic.

Likely files:

- `Assets/QuantumUser/Simulation/AnachronUnits.qtn`
- `Assets/QuantumUser/Simulation/SelectionSystem.cs`
- `Assets/QuantumUser/View/AnachronSelectablePrimitiveView.cs`
- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`

Acceptance:

- Players can select the Quill landmark.
- It does not accept player commands unless intentionally allowed.

### Sprint 25: Quill Capture Progress

Status: `[ ]`

Goal: Add ground-domain capture progress for the Quill waist.

Scope:

- Units near the Quill contribute capture.
- One owner or neutral state.
- No victory condition yet.

Likely files:

- `Assets/QuantumUser/Simulation/AnachronUnits.qtn`
- New `QuillCaptureSystem.cs`
- `Assets/QuantumUser/Simulation/SystemSetup.User.cs`
- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`

Acceptance:

- Capture progress changes deterministically.
- UI shows neutral/owned/contested state.
- Ownership survives normal gameplay until changed.

### Sprint 26: Quill Ownership Bonus

Status: `[ ]`

Goal: Give captured Quill waist one simple benefit.

Scope:

- Choose one: vision, resource trickle, production discount, or command range.
- No victory condition.

Likely files:

- `QuillCaptureSystem.cs`
- Relevant economy/HUD files depending bonus.

Acceptance:

- Bonus applies only to current Quill owner.
- Losing the Quill removes the bonus cleanly.

### Sprint 27: Quill-Based Victory Toggle

Status: `[ ]`

Goal: Add an optional prototype win condition tied to holding the Quill.

Scope:

- Hold Quill for a set deterministic duration to win.
- Keep main-base defeat still available unless explicitly replaced.

Likely files:

- `Assets/QuantumUser/Simulation/AnachronUnits.qtn`
- New or existing victory/defeat system.
- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`

Acceptance:

- A player can win by holding the Quill long enough.
- Existing main-base defeat does not regress.

## Architecture Gates From First Hand

### Sprint 28: Quantum MovementDomain Architecture Note

Status: `[ ]`

Goal: Adapt First Hand's MovementDomain direction to this Photon Quantum prototype before adding air, underwater, or orbit systems.

Scope:

- Document how Ground, Air, Underwater, and Orbit should be represented in Quantum.
- Decide whether the first version is a schema component, enum/tag, or design-only convention.
- Identify which systems must stay shared and which become domain-specific.
- No gameplay behavior changes.

Likely files:

- New `handoff/MOVEMENT_DOMAIN_QUANTUM_NOTE.md`
- `handoff/KILNFALL_SPRINT_PLAN.md`
- `handoff/PROJECT_STATE.md`

Acceptance:

- The note explains how MovementDomain maps to Quantum deterministic simulation.
- Domain expansion sprints reference the note.
- No schema or gameplay change is made in this sprint.

### Sprint 29: Ardent Concord Pathfinder Scope Note

Status: `[ ]`

Goal: Make Ardent Concord the first mission-quality faction target, matching First Hand guidance.

Scope:

- Define what "pathfinder faction" means for this repo.
- List which Ardent Concord behaviors should become production-quality first.
- Do not rebalance Wrought or Gharn in this sprint.

Likely files:

- New `handoff/ARDENT_CONCORD_PATHFINDER_SCOPE.md`
- `handoff/KILNFALL_SPRINT_PLAN.md`
- `handoff/PROJECT_STATE.md`

Acceptance:

- Future faction mechanics work has a clear first-faction priority.
- The note preserves Wrought and Gharn as prototype opponents, not abandoned factions.

### Sprint 30: Grain/Cast/Seal Data Contract Note

Status: `[ ]`

Goal: Define the reusable data model for Grain-loud, Cast, Seal, Tell, and Virii copying before implementing any Virii mechanics.

Scope:

- Document what must be tracked deterministically.
- Identify events that make a unit or building Grain-loud.
- Define first-pass Tell labels and Seal ownership concepts.
- No code or schema changes.

Likely files:

- New `handoff/GRAIN_CAST_SEAL_CONTRACT.md`
- `handoff/KILNFALL_SPRINT_PLAN.md`
- `handoff/PROJECT_STATE.md`

Acceptance:

- Virii and Tell-related sprints can point to a shared data contract.
- The note explicitly avoids one-off faction hacks.

## Grain And Identity Path

### Sprint 31: Add Grain-Loud State

Status: `[ ]`

Goal: Track when a unit exposes its record through healing, repair, upgrade, or hero rebuild.

Scope:

- Add state and timer only.
- No Virii copying yet.

Likely files:

- `Assets/QuantumUser/Simulation/AnachronUnits.qtn`
- Systems that heal, repair, upgrade, or rebuild.
- `Assets/QuantumUser/View/AnachronSelectablePrimitiveView.cs`

Acceptance:

- Relevant events mark units Grain-loud for a short deterministic duration.
- View can show a subtle indicator.

### Sprint 32: Add Faction Tell Labels

Status: `[ ]`

Goal: Add UI/lore labels for each faction's identity check.

Scope:

- Ardent Concord: Countersign.
- Wrought: Count.
- Gharn: Burr / warm brand-scars.
- No gameplay effect.

Likely files:

- `Assets/QuantumUser/View/AnachronPrototypeHud.cs`
- Faction display helper files.

Acceptance:

- Selected units/buildings can show a faction tell label.
- No simulation behavior changes.

### Sprint 33: Virii Placeholder Faction Entry

Status: `[ ]`

Goal: Add design-only or code-safe placeholder for future Virii support.

Scope:

- Document/UI-only unless faction enum expansion is explicitly chosen.
- No playable Virii yet.

Likely files:

- `handoff/KILNFALL_GAME_DIRECTION.md`
- `handoff/PROJECT_STATE.md`
- Possibly faction helper display files only.

Acceptance:

- Future Virii implementation has named hooks.
- No current three-faction gameplay changes.

## Additional Powers Expansion

### Sprint 34: Decide Faction ID Expansion Strategy

Status: `[ ]`

Goal: Choose how to support all eight Kilnfall powers.

Scope:

- Decide whether to replace current enum IDs, extend them, or keep prototype IDs and map display names.
- No code changes unless documenting the decision.

Acceptance:

- Written decision added to `handoff/PROJECT_STATE.md` or a dedicated architecture note.

### Sprint 35: Add Seethe As Fourth Power

Status: `[ ]`

Goal: Add Seethe as a selectable/assignable faction.

Scope:

- Faction ID, display names, colors, basic stats.
- No Working Set mechanic.

Likely files:

- `Assets/QuantumUser/Simulation/FactionId.cs`
- `Assets/QuantumUser/Simulation/FactionStats.cs`
- `Assets/QuantumUser/Simulation/TestSelectableBootstrapSystem.cs`
- View label/color files.

Acceptance:

- Seethe can be assigned to a player in the bootstrap scenario.
- Existing three factions still work.

### Sprint 36: Add Veirn As Fifth Power

Status: `[ ]`

Goal: Add Veirn as a selectable/assignable faction.

Scope:

- Faction ID, display names, colors, basic stats.
- No debt mechanic yet.

Acceptance:

- Veirn can be assigned to a player in the bootstrap scenario.
- Existing factions still work.

### Sprint 37: Add Vaelun As Sixth Power

Status: `[ ]`

Goal: Add Vaelun as a selectable/assignable faction.

Scope:

- Faction ID, display names, colors, basic stats.
- No Appetite mechanic yet.

Acceptance:

- Vaelun can be assigned to a player in the bootstrap scenario.
- Existing factions still work.

### Sprint 38: Add Nimhara As Seventh Power

Status: `[ ]`

Goal: Add Nimhara as a selectable/assignable faction.

Scope:

- Faction ID, display names, colors, basic stats.
- No Draw/Falls mechanic yet.

Acceptance:

- Nimhara can be assigned to a player in the bootstrap scenario.
- Existing factions still work.

### Sprint 39: Add Virii As Eighth Power

Status: `[ ]`

Goal: Add Virii as a selectable/assignable faction.

Scope:

- Faction ID, display names, colors, basic stats.
- No Rubbing/copying mechanic yet.

Acceptance:

- Virii can be assigned to a player in the bootstrap scenario.
- Existing factions still work.

## Domain Expansion

### Sprint 40: Air Domain Design Stub

Status: `[ ]`

Goal: Add design and code placeholders for air without changing gameplay.

Scope:

- Build from `handoff/MOVEMENT_DOMAIN_QUANTUM_NOTE.md`.
- Document how air units will be represented.
- Decide whether air is lane-based, layer-based, or full spatial movement.

Acceptance:

- A short architecture note exists.
- No gameplay regression.

### Sprint 41: Add First Air Scout Unit

Status: `[ ]`

Goal: Add one simple air-style unit, likely Ardent Concord Rubbing-Kite.

Scope:

- Basic movement/combat or scout-only behavior.
- Avoid full air system if not ready.

Acceptance:

- Unit can be produced or spawned.
- It does not break ground movement/pathing.

### Sprint 42: Underwater Domain Design Stub

Status: `[ ]`

Goal: Add design and code placeholders for underwater/root domain.

Scope:

- Build from `handoff/MOVEMENT_DOMAIN_QUANTUM_NOTE.md`.
- Document pressure, stealth/noise, and Quill root implications.
- No gameplay changes.

Acceptance:

- A short architecture note exists.

### Sprint 43: Add First Underwater Objective Prototype

Status: `[ ]`

Goal: Represent underwater control with a simple off-map or map-edge objective.

Scope:

- No full submarine movement yet.
- Could be an interactable root access point.

Acceptance:

- Objective exists without disrupting ground RTS flow.

### Sprint 44: Orbit Domain Design Stub

Status: `[ ]`

Goal: Add design and code placeholders for orbit/anchor domain.

Scope:

- Build from `handoff/MOVEMENT_DOMAIN_QUANTUM_NOTE.md`.
- Document command disruption and Quill anchor role.
- No gameplay changes.

Acceptance:

- A short architecture note exists.

### Sprint 45: Add First Orbital Support Prototype

Status: `[ ]`

Goal: Add one simple orbital support effect tied to Quill ownership.

Scope:

- Choose one small effect such as reveal, strike marker, or temporary production boost.

Acceptance:

- Effect is deterministic.
- It is clearly tied to ownership/control.

## Campaign And Story Layer

### Sprint 46: Add Ardent Concord Mission 1 Scope

Status: `[ ]`

Goal: Define the first mission-quality target as Ardent Concord Mission 1, "Slack Water."

Scope:

- Capture mission intent, required mechanics, environment needs, and what can remain placeholder.
- Do not implement mission logic yet.

Likely files:

- New `handoff/SLACK_WATER_SCOPE.md`
- `handoff/PROJECT_STATE.md`

Acceptance:

- The first campaign/tutorial target is written clearly enough to drive future small sprints.
- The scope waits on stable economy/construction/combat before full implementation.

### Sprint 47: Add Protagonist Placeholder Selection

Status: `[ ]`

Goal: Represent that each power has two authored leads without full campaign branching.

Scope:

- UI or config-only placeholder.
- First target: Ardent Concord Yesa/Ovid.

Acceptance:

- The prototype can show which protagonist route is selected.
- No mission branching yet.

### Sprint 48: Add Mission Objective Panel

Status: `[ ]`

Goal: Replace purely debug-style HUD with a small objective panel.

Scope:

- Show current objective: gather, build, defend, capture Quill, etc.
- No new objective logic unless it already exists.

Acceptance:

- Objective text updates from deterministic or predefined state.
- HUD remains readable.

### Sprint 49: Add First Kilnfall Tutorial Objective

Status: `[ ]`

Goal: Teach the current worker/build/supply loop using Kilnfall language.

Scope:

- Example: Build a Countersign Post / Count Relay / Tally Stone.
- No new mechanics.

Acceptance:

- Player can complete the objective using existing controls.
- Completion is detected deterministically if detection is implemented.

## Verification And Maintenance

### Sprint 50: Add Smoke Test Checklist

Status: `[x]`

Goal: Create a repeatable manual test checklist for the prototype.

Scope:

- Selection.
- Movement.
- Gathering/deposit.
- Worker production.
- Supply placement/construction.
- Cancel/refund/deconstruction.
- Combat/main-base defeat.

Likely files:

- New `handoff/SMOKE_TEST_CHECKLIST.md`

Acceptance:

- Checklist exists and can be used after each sprint.

Implementation note:

- Added `handoff/SMOKE_TEST_CHECKLIST.md`.
- Checklist covers startup, selection, movement, gathering/deposit, worker production, support placement/construction, cancel/refund/deconstruction, combat/main-base defeat, and Kilnfall presentation checks.
- No gameplay, schema, view, or build behavior was changed.

### Sprint 51: Add Automated Compile/Test Command Note

Status: `[ ]`

Goal: Document the exact local Unity/Quantum compile/test commands once confirmed.

Scope:

- Find reliable command.
- Document where logs appear.

Likely files:

- `handoff/PROJECT_STATE.md`
- `handoff/SMOKE_TEST_CHECKLIST.md`

Acceptance:

- Future tasks know how to verify compile without guessing.

### Sprint 52: Update Master Project Document

Status: `[ ]`

Goal: Bring the old Anachron master document in line with Kilnfall direction.

Scope:

- Update only project direction, faction names, and roadmap references.
- Do not rewrite unrelated architecture sections.

Likely files:

- `# RTS Master Project Document For Anachron`

Acceptance:

- Master doc no longer contradicts Kilnfall direction.
- Determinism/Quantum architecture rules remain intact.

## Recommended Immediate Order

Use this order first:

1. Sprint 1: Rename Prototype Faction Display Names.
2. Sprint 2: Rename Unit Display Labels.
3. Sprint 3: Rename Main Building Labels.
4. Sprint 4: Rename Supply Building Labels.
5. Sprint 5: Rename Resource Display Language.
6. Sprint 6: Update Faction Colors.
7. Sprint 7: Update Primitive Silhouettes.
8. Sprint 8: Update Construction Visual States.
9. Sprint 50: Add Smoke Test Checklist.
10. Sprint 9: Rename Prototype Scenario Concept.
11. Sprint 28: Quantum MovementDomain Architecture Note.
12. Sprint 29: Ardent Concord Pathfinder Scope Note.
13. Sprint 30: Grain/Cast/Seal Data Contract Note.

After that, choose between map identity work, pathing stability, or first Ardent Concord mechanics. Do not start domain expansion or Virii implementation before the relevant architecture/data-contract notes exist.
