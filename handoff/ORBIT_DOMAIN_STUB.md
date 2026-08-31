# Orbit Domain Stub

Created: 2026-08-31

Purpose: define the first Orbit/anchor-domain representation before any orbital support, anchor objective, or command-disruption behavior is added.

## Decision

Use the same layer-based deterministic `MovementDomain` model chosen for Air and Underwater/root play.

Orbit should be represented as `MovementDomain.Orbit = 3`, sharing the same strategic 2D world coordinate plane as the other domains. Altitude, orbital coverage, anchor alignment, and command interference should be deterministic gameplay rules, not Unity camera effects or scene-only objects.

Do not add a schema component in this sprint. The live prototype remains Ground plus the scout-only Air proof and inert Root Access objective until an explicit orbit gameplay sprint needs domain-aware routing.

## Orbit Representation

Orbit entities should share core Quantum contracts when they exist as selectable or targetable objects:

- `UnitIdentity` or an objective-specific identity helper.
- `Transform2D`.
- `UnitHealth` or `Targetable` where interaction/combat requires it.
- selection state when player inspection is useful.
- owner/faction lookup through player state if controlled by a faction.

Orbit should not become a separate simulation model. It should behave like another deterministic layer that can project effects onto Ground, Air, Underwater/root, and objectives.

## Movement And Coverage Shape

Orbit should start as coverage and anchor projection over the same map, not free-form 3D flight.

First-pass orbit behavior should:

- use deterministic `FPVector2` anchor or coverage centers.
- avoid full orbital pathing until a unit or support platform truly needs movement.
- describe coverage with simple radii, lanes, or map sectors.
- keep visual altitude, sky markers, beams, or projected shadows in Unity view code only.
- share player ownership, cooldowns, command input, and target validation patterns with existing systems.

Do not make orbit a second camera/game board yet. The ground map needs to remain the readable tactical truth while orbit acts as pressure, reveal, disruption, or support.

## Command Disruption

Orbit should eventually interact with commands rather than only dealing damage.

Useful first command-disruption mechanics:

- Delay enemy commands inside a projected zone.
- Scramble or reveal queued movement targets.
- Temporarily disable support-building construction in a marked radius.
- Increase cooldowns for enemy orbital or Quill pressure responses.

Any disruption must be explicit in HUD/debug feedback and deterministic in Quantum state. Avoid hidden random failure or Unity-only presentation as the source of gameplay.

## Quill Anchor Role

Quill towers can become orbital anchors without becoming instant-win towers.

Future Quill-orbit directions:

- Holding a Quill tower grants orbital targeting authority near that tower.
- Holding several Quills expands orbital coverage or reduces support cooldowns.
- Holding all major Quill towers can start a visible base-pressure effect, but opponents can stop it by recapturing one tower.
- Root Access objectives can sabotage or reveal Quill anchors from below, tying Underwater/root and Orbit together.

Main-base destruction should remain the active defeat path until a later, explicitly designed victory rule replaces or supplements it.

## First Orbital Prototype Guidance

Sprint 49 should add one simple orbital support effect, not a full orbital unit layer.

Good first candidates:

- A temporary reveal pulse centered on the owned Quill.
- A visible strike marker that damages nothing yet but proves targeting and cooldown UI.
- A short production or repair boost projected from a Quill anchor.
- A command-scramble marker that only reports debug state first.

Keep it non-disruptive:

- no full orbital movement.
- no fog-of-war dependency unless reveal is the prototype.
- no random command failure.
- no schema change unless live orbit state requires it.

## Guardrails

- Gameplay truth stays in Quantum simulation.
- Unity view scripts may only present orbital state from simulation.
- Do not fork ownership, health, selection, faction, or objective systems for Orbit.
- Do not make Orbit a purely cinematic effect if it changes gameplay.
- Do not add an orbital win condition before Quill and main-base defeat rules are stable.
