# Quantum MovementDomain Architecture Note

Created: 2026-08-28

Purpose: adapt First Hand's MovementDomain direction to this Photon Quantum prototype before adding air, underwater, or orbit gameplay.

## Decision

MovementDomain should become a deterministic simulation-owned routing contract, not a Unity view convention.

The current prototype should continue treating all active units and buildings as Ground by default. Do not add a `MovementDomain` schema component until a gameplay sprint needs more than one domain in live simulation. When that sprint arrives, prefer a small Quantum schema field or component that uses integer constants, matching the existing style of `UnitKind`, `FactionId`, and `MoveIntent.MovementMode`.

Recommended first implementation shape:

- Add `MovementDomain` integer constants in simulation code.
- Add a schema field only on entities that need domain-aware routing.
- Keep Ground as `0` for default/fallback behavior.
- Route movement, targeting, detection, production, and UI filtering from deterministic Quantum state.
- Keep Unity view scripts responsible only for presentation of domain state already produced by Quantum.

## Domain Map

Ground:

- Current default for workers, heroes, main buildings, support buildings, resource nodes, and the Quill-Waist objective.
- Uses the existing `Transform2D`, `MoveIntent`, `MovementMode`, and Quantum NavMesh/fallback movement path.
- Remains the vertical-slice priority until economy, construction, combat, repair, Quill objectives, and pathing feel stable.

Air:

- First likely expansion domain because ranked launch direction is Ground plus Air.
- Should share owner, health, selectable, targetable, command intent, and faction identity contracts with Ground.
- Should get domain-specific movement routing only when the first air unit exists.
- Must not be implemented as view-only height offsets; attackability, selection, detection, and movement permission need deterministic state.

Underwater:

- Real later pillar, but should wait until Ground plus early Air are stable.
- Should share entity identity, health, ownership, faction, production, and targeting contracts wherever possible.
- Likely needs domain-specific pathing, visibility/detection, and production routing.
- Must not fork into a separate simulation model.

Orbit:

- Real later pillar for space/orbit play.
- Should be represented as another deterministic domain value, not a separate game loop.
- Likely needs distinct camera presentation, detection rules, weapon routing, production routing, and map-layer constraints.
- Should remain blocked until the ground loop and domain contract have proven stable.

## Shared Systems

These systems should stay shared across domains unless a concrete mechanic proves they need a specialized branch:

- Ownership and faction identity.
- Health, death, cleanup, and defeat consequences.
- Selection eligibility and selected HUD identity.
- Command input ingestion.
- Resource economy and player state.
- Tech, hero, and production state where the mechanic is not domain-specific.
- Objective ownership, including Quill-style shared objectives when applicable.

Domain-specific behavior should be routed behind small deterministic checks instead of duplicating whole systems per domain.

## Domain-Specific Systems

These areas are expected to need domain-aware branches or separate systems:

- Movement/pathing: Ground uses Quantum NavMesh or straight-line fallback today; Air, Underwater, and Orbit should not inherit Ground path blockers by accident.
- Targeting: weapons may be Ground-only, Air-only, cross-domain, or objective-only.
- Detection/vision: future fog, submerged, altitude, and orbital visibility rules should read domain state.
- Production/spawning: a building or ability may create only certain domains.
- Camera and view presentation: Unity can present layers differently, but must read the authoritative domain from Quantum.
- Map constraints: domain-specific passability should be deterministic and simulation-owned.

## Current Prototype Implications

Sprint 28 makes no schema or gameplay change.

Current movement already has a useful routing precedent:

- `MoveIntent.MovementMode` stores a deterministic integer route.
- `MovementMode.StraightLineFallback` and `MovementMode.QuantumNavMesh` select the active movement system.
- `StraightLineMovementSystem` and `NavMeshMovementRequestSystem` both ignore intents outside their route.

MovementDomain should follow that pattern when needed: small integer identifiers, deterministic system routing, and no Unity-only gameplay decisions.

## Guardrails For Future Sprints

- Do not add air, underwater, or orbit gameplay before referencing this note.
- Do not create domain-only versions of core entity identity, ownership, health, selection, or faction state.
- Do not use MonoBehaviours as the source of domain truth.
- Do not make domain behavior depend on nondeterministic Unity physics, time, transforms, or scene hierarchy state.
- Do not move to a large architecture rewrite just to reserve future domains.
- Add schema only when live gameplay needs an entity to carry a domain.
- Run Quantum CodeGen for any `.qtn` schema change before expecting Unity compile verification.

## First Domain Expansion Recommendation

When domain expansion begins, start with one low-risk Air unit or drone-like entity:

1. Add a deterministic domain identifier.
2. Keep existing ownership, health, selection, targetable, and faction systems.
3. Add only the minimum movement/targeting routing needed for that unit.
4. Smoke-test Ground behavior to ensure it remains unchanged.
5. Update this note with whatever the first live domain sprint proves.
