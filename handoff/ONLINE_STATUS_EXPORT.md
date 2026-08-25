# Kilnfall Prototype Online Status Export

Export date: 2026-08-23.

Audience: First Hand Codex / online project tracker.

## Current Build Snapshot

The local Anachron prototype is being redirected into a Kilnfall ground-first RTS prototype. It currently runs as a Unity 6 view layer with Photon Quantum deterministic simulation.

The prototype already supports:

- Selection and command input.
- Right-click movement, gather, deposit, attack, health, death, and main-building defeat.
- Worker production.
- Hero lifecycle and rebuild flow.
- Three-player prototype faction setup.
- Timed worker-built support buildings with placement preview, validation, construction, cancellation/refunds, deconstruction, and capacity changes.

Current faction mapping:

- Player 0 / Tech ID -> Ardent Concord
- Player 1 / Fantasy ID -> Wrought
- Player 2 / Hybrid ID -> Gharn

The enum/internal IDs are still prototype IDs. Display names and presentation now use Kilnfall language.

## Completed Sprint Log

### Sprint 1: Faction Display Names

Status: complete.

Result: visible faction names now display as Ardent Concord, Wrought, and Gharn.

Verification: manually verified in Unity screenshot.

### Sprint 2: Unit Display Labels

Status: complete.

Result: visible worker/hero labels now display as:

- Ardent Concord: Keelwatch Ranker / Concord Marshal
- Wrought: Wright / Wrought Overseer
- Gharn: Sinterjack / Tally Captain

Verification: manually verified in Unity screenshot.

### Sprint 3: Main Building Labels

Status: complete.

Result: visible main building labels now display as:

- Ardent Concord: Ledger House
- Wrought: Longhold Node
- Gharn: Oathpyre

Verification: manually verified in Unity screenshot.

### Sprint 4: Support Building Labels

Status: complete.

Result: visible support/supply building labels now display as:

- Ardent Concord: Countersign Post
- Wrought: Count Relay
- Gharn: Tally Stone

Verification: static pass completed; next screenshot should confirm in Unity when a support building is built/selected.

### Sprint 5: Resource Display Language

Status: complete.

Result: visible resource language now uses:

- Salvage
- Plate
- Holding

Verification: manually verified in Unity screenshot using compact HUD labels `S`, `P`, and `H`.

### Sprint 6: Faction Colors

Status: complete.

Result: primitive faction colors now better match Kilnfall direction:

- Ardent Concord: grey-blue and brass-orange.
- Wrought: dark industrial yellow.
- Gharn: furnace red / raw iron.

Verification: user confirmed it worked.

### Sprint 7: Primitive Silhouettes

Status: complete.

Result: primitive scales/types were tuned so the factions read differently:

- Ardent Concord: compact, standardized forms.
- Wrought: blockier machine-like forms.
- Gharn: heavier grounded forms.

Verification: pending next visual check.

### Sprint 8: Construction Visual States

Status: complete.

Result: support-building construction states now read more like Kilnfall foundations:

- Ardent Concord foundations use a signed scaffold / tally-work color.
- Wrought foundations use a muted machine-plate color.
- Gharn foundations use an ash/furnace color.
- Deconstructing support buildings use a separate warning color and lower profile.
- Placement preview was flattened slightly so it reads as a foundation footprint.

Verification: pending next visual check.

### Sprint 50: Smoke-Test Checklist

Status: complete.

Result: added a reusable manual checklist for small sprint verification.

Checklist covers:

- Startup and HUD frame advance.
- Selection and Kilnfall labels.
- Movement.
- Gathering/deposit.
- Worker production.
- Support placement/construction.
- Cancel/refund/deconstruction.
- Combat/main-base defeat.
- Kilnfall presentation checks.

Verification: document exists and is ready for the next manual Unity run.

### Sprint 9: Prototype Scenario Concept

Status: complete.

Result: the current ground scenario now presents as `Ashenspar Quill-Waist`.

Implementation notes:

- Added one scenario display-name constant.
- Updated the HUD map row to use the Kilnfall location name while preserving resource/base/support counts.
- No spawn positions, resource amounts, map bounds, navmesh, schema, or gameplay behavior changed.

Verification: pending next visual check.

### Sprint 10: View-Only Environmental Landmarks

Status: complete.

Result: the battlefield now gets runtime-only Ashenspar landmarks.

Implementation notes:

- Added Mere boundary bands.
- Added a distant Quill waist spire/ring.
- Added shard ridge markers.
- Landmark colliders are disabled and removed.
- No scene asset, spawn/resource layout, map bounds, navmesh, schema, or gameplay behavior changed.

Verification: pending next visual check.

### Sprint 11: Resource Node Landmark Alignment

Status: complete.

Result: the upper resource pair now sits closer to the shard ridge landmarks.

Implementation notes:

- Wood moved from `(-10, 10)` to `(-14, 18)`.
- Iron moved from `(10, 10)` to `(14, 18)`.
- The other four resources were unchanged.
- Resource count, resource types, resource amounts, bases, units, map bounds, navmesh, schema, and gameplay rules were unchanged.

Verification: pending next manual smoke test.

### Sprint 12: Central Quill-Waist Objective Landmark

Status: complete.

Result: added a visible central Quill-waist objective marker.

Implementation notes:

- Added a view-only central objective cylinder and ring.
- Marker is non-colliding.
- No Quantum entity, capture state, score, resource effect, targetable component, pathing effect, or placement effect was added.
- No schema changes were made, so Quantum CodeGen was not required.

Verification: pending next visual check.

### Sprint 13: Completed Support Building Avoidance Obstacle

Status: complete.

Result: completed, healthy support buildings now receive a Quantum `NavMeshAvoidanceObstacle`.

Implementation notes:

- Obstacle radius is `1.35`.
- Foundations do not block.
- Deconstructing buildings remove the obstacle.
- Canceled, destroyed, and removed buildings do not keep the obstacle.
- Unity verification showed the avoidance obstacle alone did not stop units from moving through completed buildings.
- Added `SupplyBuildingCollisionSystem` as a deterministic fallback that pushes live units out of completed support building footprints.
- Follow-up verification showed units move around buildings, but the fallback felt like scraping/dragging along a wall.
- Added a target-aware slide bias so correction pushes units outward and slightly around completed support buildings toward their current nav target.
- Move targets clicked inside completed support building footprints are nudged outside the blocker before pathfinding.
- Video review showed units could jump quickly during correction, so collision correction is now capped at `0.12` units per tick.
- Existing placement validation was unchanged.
- No schema or CodeGen changes were required.

Verification: pending follow-up Unity smoke test for the smoother slide fallback.

### Sprint 14: Worker Build Mode Before Placement Preview

Status: complete.

Result: selecting a worker no longer shows the support-building placement shadow by itself.

Implementation notes:

- Pressing `B` with a selected owned worker enters build mode.
- Pressing `B` without a selected owned worker keeps the existing train-worker path.
- Pressing `C` while build mode is active submits the existing support-building placement intent and exits build mode.
- `Escape` or losing worker selection exits build mode.
- Placement preview appears only while build mode is active.
- No Quantum schema, placement validation, construction, cancellation, refund, or deconstruction logic changed.
- Follow-up: builder work target now uses a farther `1.85` diagonal offset so the assigned builder stands outside the support-building blocker footprint while construction starts.

Verification: pending Unity smoke test.

### Sprint 14B: Build Placement Grid Preview

Status: complete.

Result: build mode now shows a local placement grid around the support-building preview.

Implementation notes:

- Grid is view-only.
- Grid appears only while worker build mode is active.
- Placement still uses the exact cursor world position; no snapping was added.
- Grid colliders are disabled and removed.
- No schema, input intent, validation, construction, cancellation, refund, deconstruction, pathing, or simulation logic changed.

Verification: pending Unity smoke test.

### Sprint 15: Placement Failure Feedback

Status: complete.

Result: build mode now shows a concise placement status in the worker HUD.

Implementation notes:

- Reasons include insufficient Salvage/Plate, too far from builder, outside build area, too close to resource, too close to building, too close to support, and too close to unit.
- Valid placement shows `Valid placement`.
- Existing valid/invalid ghost color remains intact.
- No Quantum schema, input intent, simulation validation, placement, construction, cancellation, refund, or deconstruction logic changed.

Verification: pending Unity smoke test.

### Sprint 16: Ardent Concord Repair Identity

Status: complete.

Result: Ardent Concord now has a small passive building sustain advantage.

Implementation notes:

- Added `ArdentConcordRepairSystem`.
- Ardent Concord owned main buildings and completed support buildings mend `5` HP every `60` simulation ticks while damaged and alive.
- Foundations and deconstructing support buildings do not mend.
- Wrought and Gharn buildings are unchanged.
- Repaired building health syncs back to `Targetable`.
- Selected main and support building panels now show health as a labeled bar with larger centered HP numbers.
- World-space health bars now receive screen-space HP number labels for units, heroes, main buildings, and support buildings.
- Prototype debug damage added: select an owned building and press `V` to deal `250` test damage without destroying it.
- No Quantum schema, CodeGen, construction, faction assignment, final combat damage rules, or normal player attack behavior changed.

Verification: pending Unity smoke test with `V` debug damage on the P0 Ledger House, Ardent Concord passive repair, selected-building health readability, and world health-number readability.

### Sprint 17: Wrought Building Durability Identity

Status: complete.

Result: Wrought now has a contained building durability advantage.

Implementation notes:

- Wrought Longhold Node max HP increased to `1800`.
- Wrought Count Relay max HP increased to `650`.
- Ardent Concord and Gharn support buildings remain `500` max HP.
- Support-building max health now comes from `FactionStats`.
- Cross-faction testing support added:
  - Game view `Start As` selector on `QuantumPhase0LocalSessionController`: P0 Ardent Concord, P1 Wrought, or P2 Gharn.
  - Selecting a different start player stores the choice and reloads the active scene so Quantum boots with the correct deterministic player slot.
  - The local debug runner uses one local input slot, and deterministic selection/commands assign that input to the configured `Start As` player.
  - Camera starts near the selected player's base.
  - HUD/build preview/friendly colors now use the active local player slot instead of assuming P0.
  - Each faction now starts with three workers around its own base.
  - P0 starting workers and hero were moved next to the Ledger House.
  - Input safety follow-up: pressing `B` trains a worker only when an owned main building is selected; selected workers still use `B` for build mode.
- No Quantum schema, CodeGen, construction timing, costs, refunds, repair rules, combat damage, or destruction cleanup changed.

Verification: pending Unity smoke test that P1 start controls Wrought, P1 Longhold displays `1800/1800`, and a newly built Wrought Count Relay displays `650/650`.

## Active / Next Sprint

Recommended next sprint: Sprint 18, Gharn hold-ground combat identity.

Goal: give Wrought a contained building durability advantage.

Expected result:

- Wrought buildings become a little tougher.
- Ardent Concord and Gharn behavior remains unchanged unless explicitly documented.
- Deterministic Quantum simulation remains the source of gameplay truth.
- Local-only details stay out of online-facing updates.

## First Hand Alignment Notes

Authenticated First Hand review confirmed:

- Ground-first development remains correct.
- Ardent Concord should be treated as the first pathfinder faction for mission-quality work.
- MovementDomain-style architecture should be adapted carefully before adding air, underwater, or orbit systems.
- Grain/Cast/Seal should become a shared data contract before Virii copying or Tell mechanics.
- Space/orbit and underwater should wait until the ground economy/construction/combat loop is stable.

## Added Planning Gates

New planning gates added to the local sprint plan:

- Quantum MovementDomain Architecture Note.
- Ardent Concord Pathfinder Scope Note.
- Grain/Cast/Seal Data Contract Note.
- Ardent Concord Mission 1 / Slack Water Scope.

These gates prevent large domain/Virii/campaign work from starting too early.

## Current Known Frictions

- Unity batchmode compile was blocked in this environment by Unity licensing initialization before script compilation.
- Manual Unity screenshots have been used for recent verification.
- The HUD is visually tight; long main-building names can clip or wrap in the status panel.
- The current project has local handoff files with implementation-specific paths, so online sync should use this export file rather than raw local docs.

## Clean Next Actions

1. Verify Sprint 7 and Sprint 8 visually in Unity.
2. Use `handoff/SMOKE_TEST_CHECKLIST.md` after each sprint.
3. Smoke-test Sprint 13 pathing around completed support buildings in Unity.
4. Smoke-test Sprint 14 worker build mode.
5. Smoke-test Sprint 14B grid preview.
6. Smoke-test Sprint 15 placement status reasons.
7. Smoke-test Sprint 16 passive Ardent Concord building mend.
8. Smoke-test Sprint 17 Wrought durability values.
9. Start Sprint 18: Gharn hold-ground combat identity.
10. Keep uploading/pasting refreshed versions of this export to the online tracker after each sprint.
