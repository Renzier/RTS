# Next Chat Prompt

Use this prompt to start a fresh Codex task without spending many tokens:

```text
We are working in `/Users/douglasgordy/Documents/Anachron`, a Unity 6 + Photon Quantum RTS prototype named Anachron. The project is a Git repo on local `main`, tracking GitHub remote `origin/main` at `https://github.com/Renzier/RTS.git`.

First read these files, in this order:
1. `handoff/PROJECT_STATE.md`
2. `handoff/SOURCE_MAP.md`
3. `handoff/KILNFALL_GAME_DIRECTION.md`
4. `handoff/KILNFALL_SOURCE_REGISTRY.md`
5. `handoff/FIRST_HAND_REVIEW_NOTES.md`
6. `handoff/ONLINE_SYNC_PLAN.md`
7. `handoff/KILNFALL_SPRINT_PLAN.md`
8. `handoff/SMOKE_TEST_CHECKLIST.md`
9. `handoff/FACTION_ID_EXPANSION_STRATEGY.md`
10. `phase3_progress.md`
11. `# RTS Master Project Document For Anachron`

Important architecture rule: all gameplay logic belongs in `Assets/QuantumUser/Simulation` using Quantum deterministic APIs (`FP`, `FPVector2`, `FPVector3`). Unity view scripts in `Assets/QuantumUser/View` only display state or submit input. Do not move gameplay decisions into MonoBehaviours.

Current phase: Phase 3. Faction identity/stats/labels/primitive silhouettes/construction visuals are in, the current ground scenario presents as `Ashenspar Quill-Waist`, runtime-only Ashenspar landmarks and a deterministic selectable/capturable central Quill-waist objective with a resource trickle ownership buff and hold-to-win condition are in, the upper resource pair has been aligned near shard ridge landmarks, the map is larger, worker production works with faction-specific worker costs, and worker-built support buildings have build-mode-gated placement preview, grid preview, placement status feedback, validation, faction-specific costs/Holding values, timed construction, multi-worker builder assignment/speedup, cancellation/refunds, timed deconstruction, completed-building collision/avoidance handling, and basic worker repair. Normal click selection now picks only the nearest eligible selectable under the pointer, while drag selection remains the group-select path. The Quill win condition now requires ownership plus clearing live enemy workers/heroes out of the Quill radius; enemy presence pauses victory progress, and the captured Quill's immediate value is its ownership resource trickle. Ardent Concord buildings now have a small passive mend, Wrought buildings have a durability bump, and Gharn heroes have a first-pass hold-ground damage bonus. Grain-loud state is now in schema: worker repair, passive mend, and hero rebuild mark entities Grain-loud for 180 deterministic ticks, and the primitive view shows a subtle cyan tint. HUD Tell labels now show Countersign, Count, Burr, or Pattern for units and selected buildings, with active Grain-loud state appended. A reusable smoke-test checklist exists at `handoff/SMOKE_TEST_CHECKLIST.md`. The project direction is shifting toward Kilnfall: a four-domain RTS across ground, air, ocean, and orbit. The current prototype has four assignable powers: P0 Ardent Concord, P1 Wrought, P2 Gharn, and P3 Seethe. Virii have a document-only placeholder and remain non-playable until a later faction-expansion/codegen sprint. `handoff/FACTION_ID_EXPANSION_STRATEGY.md` chooses canonical Kilnfall integer IDs while keeping `Tech`, `Fantasy`, and `Hybrid` compatibility aliases for the first three factions. `handoff/MOVEMENT_DOMAIN_QUANTUM_NOTE.md` records how MovementDomain should map to deterministic Quantum routing before domain expansion starts, `handoff/ARDENT_CONCORD_PATHFINDER_SCOPE.md` defines Ardent Concord as the first mission-quality pathfinder faction while preserving Wrought/Gharn as prototype opponents, and `handoff/GRAIN_CAST_SEAL_CONTRACT.md` defines the shared Grain-loud/Cast/Seal/Tell contract before Virii copying.

Likely next step: follow `handoff/KILNFALL_SPRINT_PLAN.md`; Sprint 36 can add Veirn as the fifth assignable faction using `handoff/FACTION_ID_EXPANSION_STRATEGY.md`, unless the user chooses a different sprint. Recent sprints should be smoke-tested in Unity, including Sprint 16 passive Ardent Concord building mend, Sprint 17 Wrought durability, Sprint 18 Gharn hold-ground damage, Sprint 19 faction worker costs, Sprint 20 faction support-building costs/refunds, Sprint 21 faction support-building Holding values, Sprint 22 multi-worker construction, Sprint 23 worker repair, Sprint 24 selectable Quill landmark, Sprint 25 Quill capture progress, Sprint 26 Quill ownership trickle, Sprint 27 Quill hold-to-win plus the Quill contested-hold correction, Sprint 28 Quantum MovementDomain architecture note, Sprint 29 Ardent Concord pathfinder scope note, Sprint 30 Grain/Cast/Seal contract note, Sprint 31 Grain-loud state, Sprint 32 faction Tell labels, Sprint 33 Virii placeholder, Sprint 34 faction ID expansion strategy, Sprint 35 Seethe assignable faction, and Sprint 13 support-building pathing fallback feel. Keep each sprint small and verify with `handoff/SMOKE_TEST_CHECKLIST.md` before moving to the next.

Before editing, inspect the relevant simulation and view files listed in `handoff/SOURCE_MAP.md`. If `.qtn` schema changes, run Quantum CodeGen before expecting Unity to compile.
```
