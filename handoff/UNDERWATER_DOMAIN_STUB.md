# Underwater Domain Stub

Created: 2026-08-31

Purpose: define the first Underwater/root-domain representation before any submerged unit, root objective, or pathing behavior is added.

## Decision

Use the same layer-based deterministic `MovementDomain` model chosen for Air.

Underwater should be represented as `MovementDomain.Underwater = 2`, sharing the same 2D world coordinate plane as Ground and Air. Depth, murk, pressure, and root access should be gameplay rules layered on deterministic entity state, not Unity-only visual offsets.

Do not add a schema component in this sprint. The live prototype remains Ground plus the scout-only Air proof until an explicit underwater gameplay sprint needs domain-aware routing.

## Underwater Representation

Underwater entities should share the core Quantum contracts wherever possible:

- `UnitIdentity`
- `Transform2D`
- `UnitHealth`
- `Targetable`
- `MoveIntent`
- selection state
- faction and owner lookup through player state

The first live underwater entity should add only the minimum new data it actually needs. If an entity must be detected, targeted, produced, or commanded differently because it is submerged, add a small deterministic domain field/component at that point and run Quantum CodeGen.

## Movement Shape

Underwater should start as a layer on the existing 2D map, not as a separate 3D ocean simulation.

First-pass underwater movement should:

- use deterministic `FPVector2` positions.
- ignore ground-only terrain blockers unless a riverbed/root wall says otherwise.
- obey underwater-only boundaries such as lakes, rivers, mere channels, or Quill roots.
- keep visual depth, ripple, or submerged presentation in Unity view code only.
- share selection, ownership, health, faction labels, and HUD identity with other domains.

Do not make underwater fully free-roaming across the whole map at first. A constrained root-channel or lake/river route will make the domain legible without turning the prototype into a second map.

## Pressure, Stealth, And Noise

Underwater identity should be built around pressure and hidden movement:

- Pressure can define how long units can stay submerged, how deep they can operate, or how much damage/control they exert near roots.
- Stealth can let submerged units avoid normal ground targeting unless revealed by specific buildings, Quill towers, or faction mechanics.
- Noise can reveal movement, attacks, repairs, or resource work in water/root zones.

These should be deterministic integer/tick systems when implemented. Avoid timers or detection based on Unity frame time, particles, colliders, or scene objects.

## Quill Root Implications

The Quill-Waist should eventually connect to underwater/root play.

Useful future directions:

- Add root access points near water or Quill towers.
- Let underwater units contest or sabotage Quill pressure from below.
- Let holding Quill towers reveal or seal nearby root channels.
- Let lake/river control influence resource routes or base pressure.

Do not make Quill ownership an instant win condition. Underwater/root mechanics should reinforce the current rule: objectives create pressure and benefits, while actual defeat still comes through main-base destruction or a later explicitly designed win condition.

## First Underwater Prototype Guidance

Sprint 47 should add one simple underwater/root objective, not a full unit roster.

Good first candidates:

- A root access point at a map edge or river channel.
- A neutral submerged salvage node that only future underwater workers can exploit.
- A Quill-root vent that can be controlled to reveal or pressure a small area.

Keep it non-disruptive:

- no full submarine movement yet.
- no fog of war requirement.
- no new combat layer unless needed.
- no schema change unless live domain-aware behavior requires it.

## Guardrails

- Gameplay truth stays in Quantum simulation.
- Unity view scripts may only present submerged/root state from simulation.
- Do not fork identity, ownership, health, selection, or faction systems for underwater.
- Do not reuse Air behavior blindly; underwater should have constraints, pressure, and stealth/noise.
- Do not add underwater gameplay until the terrain/map and current Ground plus Air smoke tests are stable.
