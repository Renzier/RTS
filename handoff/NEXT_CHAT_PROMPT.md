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
9. `phase3_progress.md`
10. `# RTS Master Project Document For Anachron`

Important architecture rule: all gameplay logic belongs in `Assets/QuantumUser/Simulation` using Quantum deterministic APIs (`FP`, `FPVector2`, `FPVector3`). Unity view scripts in `Assets/QuantumUser/View` only display state or submit input. Do not move gameplay decisions into MonoBehaviours.

Current phase: Phase 3. Faction identity/stats/labels/primitive silhouettes/construction visuals are in, the current ground scenario presents as `Ashenspar Quill-Waist`, runtime-only Ashenspar landmarks and a deterministic selectable/capturable central Quill-waist objective with a resource trickle ownership bonus and hold-to-win condition are in, the upper resource pair has been aligned near shard ridge landmarks, the map is larger, worker production works with faction-specific worker costs, and worker-built support buildings have build-mode-gated placement preview, grid preview, placement status feedback, validation, faction-specific costs/Holding values, timed construction, multi-worker builder assignment/speedup, cancellation/refunds, timed deconstruction, completed-building collision/avoidance handling, and basic worker repair. Ardent Concord buildings now have a small passive mend, Wrought buildings have a durability bump, and Gharn heroes have a first-pass hold-ground damage bonus. A reusable smoke-test checklist exists at `handoff/SMOKE_TEST_CHECKLIST.md`. The project direction is shifting toward Kilnfall: a four-domain RTS across ground, air, ocean, and orbit, with early prototype factions mapped from Tech/Fantasy/Hybrid to Ardent Concord/Wrought/Gharn. `handoff/MOVEMENT_DOMAIN_QUANTUM_NOTE.md` now records how MovementDomain should map to deterministic Quantum routing before domain expansion starts.

Likely next step: follow `handoff/KILNFALL_SPRINT_PLAN.md`, starting with Sprint 29 unless the user chooses a different sprint. Recent sprints should be smoke-tested in Unity, including Sprint 16 passive Ardent Concord building mend, Sprint 17 Wrought durability, Sprint 18 Gharn hold-ground damage, Sprint 19 faction worker costs, Sprint 20 faction support-building costs/refunds, Sprint 21 faction support-building Holding values, Sprint 22 multi-worker construction, Sprint 23 worker repair, Sprint 24 selectable Quill landmark, Sprint 25 Quill capture progress, Sprint 26 Quill ownership trickle, Sprint 27 Quill hold-to-win, Sprint 28 Quantum MovementDomain architecture note, and Sprint 13 support-building pathing fallback feel. Keep each sprint small and verify with `handoff/SMOKE_TEST_CHECKLIST.md` before moving to the next.

Before editing, inspect the relevant simulation and view files listed in `handoff/SOURCE_MAP.md`. If `.qtn` schema changes, run Quantum CodeGen before expecting Unity to compile.
```
