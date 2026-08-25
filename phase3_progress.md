# Phase 3 Progress Log

## 2026-08-22 - Phase 3 Kickoff

Baseline entering Phase 3:
- Phase 1 core input, selection, right-click movement, NavMesh movement, economy loop, and defeat cleanup are working.
- Phase 2 tech upgrades, hero lifecycle, rebuild timers, combat, damage, health bars, building selection, and HUD feedback are working.
- Architecture rule remains strict: deterministic gameplay state belongs in `Assets/QuantumUser/Simulation`; Unity view scripts only display or submit input.

Phase 3 target from the architecture document:
- Replace generic primitives with Tech, Fantasy, and Hybrid unit data.
- Implement dynamic NavMesh obstacle baking for placed buildings.

Current Phase 3 step:
- Add deterministic faction identity for each player as the first layer of faction implementation.
- Surface faction labels/colors in the Unity view without moving gameplay logic out of Quantum.

Completed in this step:
- Added `FactionId` constants in Simulation for Tech, Fantasy, and Hybrid.
- Added `PlayerFactionState` to the Quantum schema.
- Assigned prototype players to factions: P0 Tech, P1 Fantasy, P2 Hybrid.
- Bootstrap now creates deterministic faction state entities during frame initialization.
- HUD player rows now show faction names.
- Primitive unit/base colors now read deterministic faction state so Phase 3 can replace generic visuals incrementally.

Verification notes:
- Because `AnachronUnits.qtn` changed, Quantum CodeGen must be run before Unity can compile this step.

## 2026-08-22 - Faction Stat Profiles

Completed in this step:
- Added `FactionStats` as the deterministic source of prototype faction tuning.
- Replaced generic worker HP, hero HP, hero damage, and main base HP with faction-specific values.
- Tech now has sturdier workers/base and stronger tier scaling.
- Fantasy now has lower durability but higher initial hero damage.
- Hybrid now sits between the two with balanced hero scaling.
- Hero rebuild now restores the correct faction-specific max health.
- Hero tech damage refresh now reads faction-specific damage curves.

Prototype tuning values:
- Tech: Worker HP 115, Hero HP 330, Hero DMG 42 +18/tier, Main Base HP 1650.
- Fantasy: Worker HP 90, Hero HP 280, Hero DMG 55 +12/tier, Main Base HP 1350.
- Hybrid: Worker HP 105, Hero HP 310, Hero DMG 48 +15/tier, Main Base HP 1500.

## 2026-08-22 - Faction Unit Display Names

Completed in this step:
- Replaced generic HUD unit labels with faction-specific prototype names.
- Tech units display as `Engineer` and `Commander`.
- Fantasy units display as `Acolyte` and `Archon`.
- Hybrid units display as `Artificer` and `Spellblade`.

Notes:
- This is view-only naming for readability. The deterministic unit kind remains `Worker` or `Hero` in Quantum.

## 2026-08-22 - Faction Prototype Silhouettes

Completed in this step:
- Replaced one-size-fits-all cylinders with faction-specific prototype primitive shapes.
- Tech units and bases now use angular cube silhouettes.
- Fantasy units use spheres and Fantasy bases use capsules.
- Hybrid units use capsule silhouettes and Hybrid bases keep the rounded cylinder style.

Notes:
- This is Unity view-only presentation. Quantum gameplay state and collision/pathing data are unchanged.

## 2026-08-22 - Larger Prototype Map and Worker Production

Completed in this step:
- Expanded the prototype scenario layout so player bases start farther apart.
- Added four more resource nodes, for six total nodes across the larger test map.
- Expanded camera pan bounds to cover the larger map.
- Updated the prototype ground setup scale from 40x40 to roughly 80x80 Unity units.
- Added deterministic worker production using `B`.
- Worker production costs 50 Wood, 25 Iron, and 1 Food.
- New workers spawn beside the selected owned main building, or beside the first living owned main building if none is selected.

Unity follow-up:
- Rebuild/bake the Quantum map/navmesh after this change so units can path to the farther nodes.

## 2026-08-22 - Worker Deposit Range Fix

Completed in this step:
- Increased worker deposit range from `1.25` to `2.25`.
- This matches the larger prototype main building footprint so newly trained workers can reliably deposit when they return to base.

Reason:
- Trained workers were gathering and returning, but could visually reach the base without crossing the previous deterministic deposit radius.

## 2026-08-22 - Worker Built Supply Building

Completed in this step:
- Added a deterministic `SupplyBuilding` component.
- Added worker construction using `C`.
- A selected living worker can place a supply building at the cursor if it is within build range.
- Supply buildings cost 100 Wood and 50 Iron.
- Supply buildings add 5 Food cap immediately.
- Supply buildings are selectable, targetable, and have 500 HP.

Controls:
- Select a worker, hover a build location near it, then press `C`.

Notes:
- This is instant prototype construction. Timed build queues and placement previews can come later.
- Because `AnachronUnits.qtn` changed, Quantum CodeGen must be run before Unity can compile this step.

## 2026-08-22 - Supply Placement Validation and Preview

Completed in this step:
- Added deterministic placement validation for supply buildings.
- Supply buildings can no longer be placed outside the map bounds.
- Supply buildings can no longer overlap resources, main bases, existing supply buildings, or units.
- Added a Unity view-only placement ghost when a worker is selected.
- The ghost turns green for valid placement and red for invalid placement.

Notes:
- The authoritative allow/deny rule is in Quantum Simulation.
- The ghost is only visual feedback and does not affect deterministic state.

## 2026-08-22 - Timed Supply Construction

Completed in this step:
- Changed worker-built supply from instant completion to deterministic timed construction.
- Supply foundations now take 600 simulation ticks, currently treated as roughly 10 seconds.
- Food cap is granted only when construction completes.
- Foundations start with minimal HP and become full-health supply buildings on completion.
- Added a selected supply/foundation HUD panel with remaining construction time and a progress bar.
- Added always-visible in-world countdown/progress feedback above each supply foundation.
- Added a distinct foundation visual scale/color so in-progress supply does not look complete.

Controls:
- Select a worker, place supply with `C`, then select the foundation to watch the timer.

Notes:
- This changed `AnachronUnits.qtn`, so Quantum CodeGen must be run before Unity can compile.

## 2026-08-22 - Supply Selection Fix

Completed in this step:
- Fixed completed supply buildings not deconstructing because `SelectionSystem` did not treat `SupplyBuilding` entities as owned selectable objects.
- Supply buildings now participate in deterministic selection ownership and dead/destroyed filtering.
- Pressing `X` after selecting a completed supply should now start the timed 80% refund deconstruction path.
- Construction is still autonomous after placement; worker channeling/cancel/refund can be added later if needed.

## 2026-08-22 - Worker Construction Assignment

Completed in this step:
- Added deterministic `WorkerBuildIntent` state.
- The selected worker who places a supply foundation is assigned as its builder.
- Assigned builders stop gathering, depositing, attacking, and accepting new move/resource commands while construction is active.
- Builders move to a nearby construction position and report `Building supply` in the player unit HUD.
- Builder assignment clears when the supply completes, is destroyed, or the worker dies/is defeated.

Notes:
- This changed `AnachronUnits.qtn`, so Quantum CodeGen must be run before Unity can compile.
- This is still a simple prototype assignment; later phases can add multi-worker build speed, cancel/refund, and repair behavior.

## 2026-08-22 - Construction Destruction Refund

Completed in this step:
- Supply foundations now store their original wood and iron cost.
- Destroying an unfinished supply foundation refunds 100% of its stored cost to the owning player.
- Refunds are guarded so each foundation can only refund once.
- Selected unfinished foundations can be cancelled with `X` for a full refund.
- Selecting the assigned builder worker and pressing `X` also cancels that worker's active foundation.
- Selected completed supply buildings can be deconstructed with `X` for an 80% refund.
- Completed supply deconstruction removes the provided food cap.
- Selected supply HUD now shows the cancel/deconstruct command and refund amount.
- Selected builder HUD now swaps to a cancel prompt while the worker is building.

Notes:
- This initial refund pass reused existing generated fields. The later timed deconstruction pass below adds new `.qtn` fields and requires Quantum CodeGen.

## 2026-08-22 - Timed Supply Deconstruction

Completed in this step:
- Completed supply buildings no longer disappear instantly when `X` is pressed.
- Pressing `X` on a completed supply starts a deterministic 300 tick deconstruction timer.
- The selected supply panel shows deconstruction time remaining and refund amount.
- The world timer/progress bar above supply now supports both construction and deconstruction.
- At timer completion, supply refunds 80% of its stored cost, removes its food cap, and is destroyed.
- Unfinished construction cancellation remains instant and fully refunded.

Notes:
- This changed `AnachronUnits.qtn`, so Quantum CodeGen must be run before Unity can compile.
