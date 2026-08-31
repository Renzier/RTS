# Air Domain Stub

Created: 2026-08-30

Purpose: define the first Air-domain representation before any live air unit or movement behavior is added.

## Decision

Use a layer-based deterministic `MovementDomain` model.

Recommended first values:

- `Ground = 0`
- `Air = 1`
- `Underwater = 2`
- `Orbit = 3`

Do not add the schema component in this sprint. The current prototype remains Ground-only in live simulation until Sprint 41 or another explicit gameplay sprint creates the first air unit.

## Air Representation

Air units should be normal Quantum entities with the same shared contracts as ground units:

- `UnitIdentity`
- `Transform2D`
- `UnitHealth`
- `Targetable`
- `MoveIntent`
- selection state
- faction and owner lookup through player state

The future domain field should be a small deterministic integer on entities that need domain-aware routing. Ground should remain the default/fallback value so existing workers, heroes, buildings, resource nodes, and Quill objective behavior do not need migration churn.

## Movement Shape

Air should start as layer-based movement over the same 2D map plane, not a separate 3D space.

First-pass air movement should:

- ignore ground-only path blockers.
- keep deterministic `FPVector2` positions.
- use Unity height/visual offset only as presentation.
- share selection, health, ownership, faction labels, and HUD identity with ground units.

Do not make air lane-based yet. Lanes may become useful for missions, formations, or carrier/orbit interactions later, but a lane model would be too restrictive for the first scout-style unit.

Do not make air full spatial 3D yet. The current combat, command, camera, and Quantum routing contracts are 2D; full spatial movement should wait until a proven need exists.

## First Air Unit Guidance

Sprint 41 should add one small scout-style Air unit as a proof. The likely path is Ardent Concord's Rubbing-Kite.

That sprint should add only the minimum needed:

- deterministic Air domain constant.
- schema field/component only if the unit needs live domain-aware routing.
- one bootstrap or production path for the scout.
- view height/shape/color presentation that reads as airborne.
- targeting guardrails so ground combat does not accidentally break.

## Guardrails

- Keep gameplay truth in Quantum simulation.
- Keep Unity view scripts presentation-only.
- Do not fork identity, ownership, health, selection, faction, or HUD systems for Air.
- Do not make existing ground entities opt into new domain behavior unless required.
- Smoke-test current ground movement, selection, combat, Quill capture, and support-building construction after the first live Air sprint.
