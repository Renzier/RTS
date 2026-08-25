# Anachron Project State

## Snapshot

- Project path: `/Users/douglasgordy/Documents/Anachron`
- Engine stack: Unity 6 view layer + Photon Quantum deterministic simulation.
- Repo status: this folder did not report as a git repository via `git status`.
- Core design: micro-heavy RTS prototype with up to three players, Tech/Fantasy/Hybrid factions, workers, heroes, resources, tech tiers, main base defeat, and supply construction.

## Non-Negotiable Architecture

- Gameplay state and decisions must live in `Assets/QuantumUser/Simulation`.
- Unity scripts in `Assets/QuantumUser/View` must only read Quantum state, present visuals/HUD, or submit input.
- Avoid nondeterministic Unity APIs in simulation code.
- Use Quantum fixed-point types and deterministic APIs in simulation code.
- `.qtn` schema edits require Quantum CodeGen before Unity compile expectations are meaningful.

## Completed Baseline

- Selection and command input are wired through Quantum input.
- Right-click movement, NavMesh movement requests, gather/deposit loops, economy state, combat, health, death cleanup, and main-base defeat exist.
- Tech upgrades, building tiers, singleton hero lifecycle, hero rebuild, and HUD feedback exist.
- Phase 3 faction foundation exists:
  - Faction IDs: Tech, Fantasy, Hybrid.
  - Player faction state entities are bootstrapped deterministically.
  - Prototype player assignment: P0 Tech, P1 Fantasy, P2 Hybrid.
  - Faction-specific worker HP, hero HP, hero damage scaling, and main base HP.
  - Faction-specific display names and primitive silhouettes in the view layer.

## Most Recent Work

- Added online sync support:
  - `handoff/ONLINE_SYNC_PLAN.md` defines how local sprint/project state should sync to The First Hand / online tracker without leaking local paths or credentials.
  - `handoff/ONLINE_STATUS_EXPORT.md` is the first clean drop-in status snapshot for the online project board.
- Kilnfall Sprint 17 completed:
  - Wrought main building durability increased: Longhold Node max HP is now `1800`.
  - Wrought support building durability increased: Count Relay max HP is now `650`.
  - Ardent Concord and Gharn support buildings remain `500` max HP.
  - New support buildings now pull max HP from `FactionStats`.
  - Cross-faction testing support added:
    - `QuantumPhase0LocalSessionController` now shows a Game view `Start As` selector: P0 Ardent Concord, P1 Wrought, or P2 Gharn.
    - Selecting a different start player stores the choice and reloads the active scene so Quantum boots with the correct deterministic player slot.
    - The local debug runner uses one local input slot, and deterministic selection/commands assign that input to the configured `Start As` player.
    - Camera starts near the selected player's base.
    - HUD/build preview/friendly colors now use the active local player slot instead of assuming P0.
    - Each faction now starts with three workers around its own base.
    - P0 starting workers and hero were moved next to the Ledger House.
    - Input safety follow-up: pressing `B` trains a worker only when an owned main building is selected; selected workers still use `B` for build mode.
  - No Quantum schema, CodeGen, construction timing, costs, refunds, repair rules, combat damage, or destruction cleanup changed.
- Kilnfall Sprint 16 completed:
  - Added `ArdentConcordRepairSystem`.
  - Ardent Concord owned main buildings and completed support buildings mend `5` HP every `60` simulation ticks while damaged and alive.
  - Foundations and deconstructing support buildings do not mend.
  - Wrought and Gharn buildings are unchanged.
  - Repaired building health syncs back to `Targetable`.
  - Follow-up HUD pass: selected main and support buildings now show health as a labeled bar with larger centered HP numbers, and world-space health bars now receive screen-space HP number labels.
  - Added prototype debug damage: select an owned building and press `V` to deal `250` test damage without destroying it, making passive repair easy to verify.
  - No Quantum schema, CodeGen, construction, faction assignment, final combat damage rules, or normal player attack behavior changed.
- Kilnfall Sprint 15 completed:
  - Build mode now shows a concise placement status in the worker HUD.
  - Reasons include insufficient Salvage/Plate, too far from builder, outside build area, too close to resource, too close to building, too close to support, and too close to unit.
  - Valid placement shows `Valid placement`.
  - Existing valid/invalid ghost color remains intact.
  - No Quantum schema, input intent, simulation validation, placement, construction, cancellation, refund, or deconstruction logic changed.
- Kilnfall Sprint 14B completed:
  - Added a view-only local grid around the support-building placement preview.
  - The grid appears only while worker build mode is active.
  - Placement still uses the exact cursor world position; no snapping was added.
  - Grid lines have disabled/removed colliders and do not affect selection, placement, pathing, construction, or Quantum simulation.
- Kilnfall Sprint 14 completed:
  - Worker selection alone no longer shows the support-building placement shadow.
  - Pressing `B` with a selected owned worker enters build mode.
  - Pressing `B` without a selected owned worker keeps the existing train-worker input path.
  - Pressing `C` while build mode is active submits the existing support-building placement intent and exits build mode.
  - `Escape` or losing worker selection exits build mode.
  - Placement preview appears only while build mode is active.
  - No Quantum schema, placement validation, construction, cancellation, refund, or deconstruction logic changed.
- Support construction follow-up:
  - Builder work target moved farther from the new foundation, from the old near-center `(-0.8, -0.8)` offset to a named `1.85` diagonal work offset.
  - This should keep the assigned builder outside the completed support-building blocker footprint while construction starts.
- Kilnfall Sprint 13 completed:
  - Completed, healthy support buildings now receive a `NavMeshAvoidanceObstacle` with radius `1.35`.
  - Foundations, deconstructing buildings, canceled foundations, destroyed buildings, and removed buildings do not keep the obstacle.
  - Unity verification showed the avoidance obstacle alone did not stop units from moving through completed buildings.
  - Added `SupplyBuildingCollisionSystem` as a deterministic fallback that pushes live units out of completed support building footprints.
  - Follow-up verification showed units move around buildings, but the fallback felt like scraping/dragging along a wall.
  - Added a target-aware slide bias so collision correction pushes units outward and slightly around completed support buildings toward their current nav target.
  - Move targets clicked inside completed support building footprints are nudged outside the blocker before pathfinding.
  - Video review showed units could jump quickly during correction, so collision correction is now capped at `0.12` units per tick.
  - Existing placement validation remains unchanged.
  - No schema or CodeGen changes were required.
  - Runtime pathing needs another Unity smoke test to confirm the fallback feels acceptable.
- Kilnfall Sprint 12 completed:
  - Added a view-only central Quill-waist objective marker and ring.
  - The marker has no collider, Quantum entity, capture state, score, resource effect, targetable component, pathing effect, or placement effect.
  - No schema changes were made, so Quantum CodeGen was not required.
- Kilnfall Sprint 11 completed:
  - Moved the upper resource pair closer to the shard ridge landmarks.
  - Wood moved from `(-10, 10)` to `(-14, 18)`.
  - Iron moved from `(10, 10)` to `(14, 18)`.
  - Resource count, resource types, resource amounts, bases, units, map bounds, navmesh, schema, and gameplay rules were intentionally left unchanged.
- Kilnfall Sprint 10 completed:
  - Added runtime-only Ashenspar view landmarks: Mere boundary bands, a distant Quill waist spire/ring, and shard ridge markers.
  - Landmark colliders are disabled and removed.
  - Scene asset, spawn/resource layout, map bounds, navmesh, schema, and gameplay behavior were intentionally left unchanged.
- Kilnfall Sprint 9 completed:
  - The current ground scenario now presents as `Ashenspar Quill-Waist` in the HUD.
  - Added `AnachronPrototypeScenario.ScenarioName` for a single source of scenario display naming.
  - Spawn positions, resource amounts, map bounds, navmesh, schema, and gameplay behavior were intentionally left unchanged.
- Kilnfall Sprint 50 completed:
  - Added `handoff/SMOKE_TEST_CHECKLIST.md` as the repeatable manual verification routine for small sprints.
  - Checklist covers startup, selection, movement, gathering/deposit, worker production, support placement/construction, cancel/refund/deconstruction, combat/main-base defeat, and Kilnfall presentation checks.
  - No gameplay, schema, view, or build behavior was changed.
- Kilnfall Sprint 8 completed:
  - Support-building construction states now read more like Kilnfall foundations.
  - Ardent Concord, Wrought, and Gharn foundations use distinct view-only colors.
  - Deconstructing support buildings use a separate warning color and lower profile.
  - The placement preview was flattened slightly to read as a foundation footprint.
  - Construction timing, refunds, health, placement validation, schema, and gameplay behavior were intentionally left unchanged.
- Kilnfall Sprint 7 completed:
  - Primitive silhouettes/scales now better distinguish Ardent Concord, Wrought, and Gharn.
  - Changes were limited to view primitive types/scales; gameplay, schema, selection logic, and health behavior were intentionally left unchanged.
- Kilnfall Sprint 6 completed:
  - Primitive faction colors now better match Ardent Concord, Wrought, and Gharn visual direction.
  - Selection, health, placement validity, schema, and gameplay behavior were intentionally left unchanged.
- Kilnfall Sprint 5 completed:
  - Visible resource labels now use Salvage, Plate, and Holding.
  - `ResourceKind`, economy state fields, costs, gathering, deposits, and capacity behavior were intentionally left unchanged.
- Revised `handoff/KILNFALL_SPRINT_PLAN.md` after First Hand review:
  - Kept Sprints 1-8 as low-risk presentation/environment conversion.
  - Added architecture/data-contract gates before later high-risk work: Quantum MovementDomain note, Ardent Concord pathfinder scope, and Grain/Cast/Seal contract.
  - Moved domain expansion and Virii-related work behind those gates.
  - Added Ardent Concord Mission 1 / "Slack Water" scope before campaign implementation.
- Reviewed authenticated First Hand source:
  - Added `handoff/FIRST_HAND_REVIEW_NOTES.md`.
  - Confirmed First Hand as the internal/spoiler-full Kilnfall codex containing lore, Codex/game design, wiki, gallery, audio, progress, and studio sections.
  - Captured build-direction implications: ground-first sequence, Ardent Concord as pathfinder faction, MovementDomain-style architecture to adapt carefully to Quantum, and future Grain/Cast/Seal data-contract needs.
- Added `handoff/KILNFALL_SOURCE_REGISTRY.md`:
  - Registered `https://thefirsthand.kilnfall.com/` as an authenticated Kilnfall source using account identifier `doug@eloai.co`.
  - No password or credential was stored.
- Kilnfall Sprint 4 completed:
  - Visible supply building labels now map the first three Kilnfall powers to Countersign Post, Count Relay, and Tally Stone.
  - Construction, cancellation, refund, placement validation, deconstruction, and food-cap behavior were intentionally left unchanged.
- Kilnfall Sprint 3 completed:
  - Visible main building labels now map the first three Kilnfall powers to Ledger House, Longhold Node, and Oathpyre.
  - Main building health, tiers, upgrade behavior, and defeat logic were intentionally left unchanged.
- Kilnfall Sprint 2 completed:
  - Visible unit display labels now map the first three Kilnfall powers to Keelwatch Ranker / Concord Marshal, Wright / Wrought Overseer, and Sinterjack / Tally Captain.
  - UnitKind values, stats, production, selection, movement, gathering, combat, and hero lifecycle behavior were intentionally left unchanged.
- Kilnfall Sprint 1 completed:
  - Visible faction display names now map Tech -> Ardent Concord, Fantasy -> Wrought, and Hybrid -> Gharn.
  - The HUD and player-state debug overlay show the Kilnfall faction names.
  - Enum IDs, schema fields, player assignment, stats, and gameplay behavior were intentionally left unchanged.
  - Unity batchmode compile was attempted via `unity-sprint1-compile.log`, but Unity failed during licensing initialization before script compilation. Manual Unity verification was provided by screenshot showing the updated in-game HUD.
- Larger prototype map layout and wider camera bounds.
- Six total resource nodes.
- Worker production using `B`.
- Worker-built supply building using `C`.
- Supply placement validation against map bounds, resources, bases, supplies, and units.
- View-only placement ghost with valid/invalid colors.
- Timed supply construction:
  - Foundations take 600 simulation ticks.
  - Food cap is granted only on completion.
  - Foundations have minimal HP until complete.
  - HUD and world progress feedback show construction status.
- Builder assignment:
  - The selected worker becomes assigned builder.
  - Builder stops gathering/depositing/attacking and ignores normal commands while building.
  - Builder assignment clears on completion, destruction, worker death, or defeat cleanup.
- Refund/deconstruction flow:
  - Unfinished foundations store original wood/iron cost.
  - Destroyed or canceled unfinished foundations refund 100%.
  - Completed supply buildings can deconstruct with `X`.
  - Completed supply deconstruction takes 300 ticks and refunds 80%.
  - Food cap is removed when completed supply deconstruction finishes.

## Controls Known From Current Prototype

- Select units/buildings with existing selection controls.
- Right-click commands move/gather/attack depending target.
- `B`: train worker from owned main building.
- `C`: selected worker places supply building near cursor if valid.
- `X`: cancel unfinished supply/foundation, cancel selected builder's active foundation, or start completed supply deconstruction.

## Likely Next Tasks

- Add Wrought building durability identity.
- Add automated compile/test command notes once the git-backed/local Unity environment is ready.
- Improve build placement preview and snap/grid behavior if needed.
- Add multi-worker build speed, repair, cancel timing, or worker channeling.
- Replace primitive visuals with stronger faction-specific assets.
- Add more faction-specific unit data beyond worker/hero/main/supply.

## Verification Notes

- Existing progress log repeatedly notes `.qtn` schema changes in Phase 3, so CodeGen may be required before compile.
- Useful logs at project root include `unity-phase0-import.log`, `unity-phase1-input.log`, `unity-quantum-import.log`, `unity-quantum-compile.log`, and `unity-clean-after-samples.log`.
