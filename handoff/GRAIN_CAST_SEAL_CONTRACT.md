# Grain/Cast/Seal Data Contract

Created: 2026-08-29

Purpose: define the reusable deterministic contract for Grain-loud state, Cast, Seal, Tell checks, and future Virii copying before any Virii mechanics are implemented.

## Decision

Grain/Cast/Seal must be modeled as shared Quantum simulation data, not as one-off faction behavior.

The Virii rule is the pressure test: they can copy exposed recorded form, but they cannot copy true belonging. That means future copy, disguise, detection, repair, healing, upgrade, hero rebuild, and faction-authenticity mechanics all need to read the same deterministic state.

Do not add gameplay implementation in this sprint. This note defines the contract future schema/code sprints should follow.

## Terms

Grain:

- The underlying reality-record exposed by repair, healing, transformation, promotion, rebuild, copying, or power-channeling.
- In code, this should appear first as a deterministic exposure state/timer on relevant entities.

Grain-loud:

- A temporary state where an entity's record is exposed enough to be observed, copied, disrupted, or checked.
- Should be deterministic and tick-based.
- Should not depend on Unity particle effects, animation state, wall-clock time, or view-only markers.

Cast:

- Copyable form: unit/building kind, visible silhouette, broad combat role, current domain, and selected stats that a copy mechanic is allowed to reproduce.
- Cast is not identity. A copied Cast may look or behave similarly, but it must still fail checks that require true belonging.

Seal:

- True ownership/belonging/authenticity.
- Should be represented as deterministic state tied to faction, source owner, original faction identity, or later oath/countersignature systems.
- A copy can carry a copied Cast but should not automatically receive the original Seal.

Tell:

- A deterministic label/check that exposes whether an entity's Cast and Seal agree.
- First-pass Tell labels should be simple, readable, and shared across factions.
- Tell checks should support future UI/HUD indicators, detection rules, and mission scripting.

## First-Pass Data Model

Recommended schema direction for a later implementation sprint:

- `GrainState` component or equivalent fields:
  - `Boolean IsGrainLoud`
  - `Int32 GrainLoudTicksRemaining`
  - `Int32 GrainLoudSource`
- `IdentitySeal` component or equivalent fields:
  - `Int32 OriginalFactionId`
  - `Int32 SealOwnerPlayer`
  - `Boolean HasTrueSeal`
- `CastRecord` component or equivalent fields only when copying begins:
  - `Int32 CastUnitKind`
  - `Int32 CastFactionId`
  - `Int32 CastMovementDomain`
  - `Int32 CastSourceEntityId` or a deterministic source reference if safe
- `TellState` component or computed result:
  - `Int32 TellLabel`
  - `Boolean CastMatchesSeal`

Keep this small at first. Add only the fields needed by the next live sprint and run Quantum CodeGen for any `.qtn` schema change.

## Grain-Loud Sources

These events should be candidates for Grain-loud exposure:

- Worker repair pulses.
- Passive building mend effects.
- Future healing effects.
- Tech upgrade start or completion.
- Building tier sync/upgrade completion.
- Hero rebuild start or completion.
- Hero level, promotion, or transformation events.
- Future domain transition effects.
- Future power-channeling, Quill-root, Tell, or Seal interactions.
- Future Virii Rubbing/Sounding observation.

Do not mark every damage event Grain-loud by default. Damage can expose records later if design needs it, but the first pass should focus on repair/heal/upgrade/rebuild events that already imply record rewriting or restoration.

## First-Pass Tell Labels

Recommended integer labels:

- `None = 0`: no Tell state is known or relevant.
- `TrueSeal = 1`: Cast and Seal agree.
- `Unsealed = 2`: entity has usable Cast but lacks a true Seal.
- `FalseCast = 3`: entity presents a Cast that does not match its Seal.
- `GrainLoud = 4`: entity is currently exposing its record.
- `Unheld = 5`: future Ardent Concord paired/countersignature failure state.

These are labels for deterministic systems and HUD/debug presentation. They are not final lore taxonomy.

## Seal Ownership Concepts

First-pass Seal ownership should answer:

- Which faction does this entity truly belong to?
- Which player currently owns or commands it?
- Is this entity original, copied, transformed, rebuilt, or unsealed?
- Does this entity qualify for faction-specific mechanics?

Important rule: gameplay systems that require authentic faction belonging should check Seal, not only visible faction/Cast.

Examples:

- Ardent Concord passive mend should eventually require true Ardent Seal, not merely Ardent-looking Cast.
- Virii copies may copy a Keelwatch Ranker Cast but should not automatically count as truly Ardent for countersignature mechanics.
- A future Tell ability should be able to reveal Cast/Seal mismatch without needing Virii-specific branches in every system.

## Virii Copying Guardrails

Future Virii mechanics should:

- Observe only deterministic Grain-loud opportunities.
- Copy Cast through a shared `CastRecord` path.
- Preserve the copy's own ownership and Seal state separately from copied appearance.
- Fail or degrade when a mechanic requires true Seal.
- Use Tell labels/checks instead of bespoke "is fake" flags scattered across systems.

Future Virii mechanics should not:

- Read Unity view state to decide what can be copied.
- Copy faction-specific privileges by default.
- Add per-faction copy exceptions in unrelated systems.
- Treat copied visuals as authoritative gameplay identity.

## Current Repo Touchpoints

Likely systems that will later mark Grain-loud:

- `WorkerRepairSystem`
- `ArdentConcordRepairSystem`
- `TechUpgradeSystem`
- `BuildingTierSyncSystem`
- `HeroRebuildSystem`
- `HeroLifecycleSystem`

Likely systems that will later read Seal/Tell:

- Faction-specific systems such as `ArdentConcordRepairSystem`
- Combat targeting and future detection systems
- Selection/HUD debug presentation
- Future Virii copying/infiltration systems
- Future mission objective scripting

## Implementation Order

Recommended next steps:

1. Add Grain-loud state/timer only.
2. Add subtle view/debug presentation for Grain-loud.
3. Add first-pass Tell labels with no Virii copying.
4. Add a Virii placeholder faction entry.
5. Add a minimal copy observation mechanic only after the shared state works.

Each step should be a small sprint with its own verification. This contract should be updated when live implementation proves a field should change.
