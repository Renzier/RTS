# Ardent Concord Pathfinder Scope

Created: 2026-08-29

Purpose: define what it means for Ardent Concord to be the first mission-quality faction target in this Photon Quantum prototype.

## Decision

Ardent Concord is the pathfinder faction for production-quality work.

In this repository, "pathfinder faction" means the first faction used to prove each major RTS loop to mission quality before that loop is generalized, mirrored, or made asymmetrical for the full roster. Ardent Concord should be the faction where input feel, readability, deterministic rules, HUD feedback, build flows, combat feedback, repair/sustain identity, and first mission goals become dependable enough to use as a standard for later faction work.

This does not mean Wrought or Gharn are abandoned. They remain active prototype opponents and contrast factions. They should continue to receive small identity passes when needed for tests, but broad production polish should land on Ardent Concord first.

## Current Mapping

- `FactionId.Tech` remains the deterministic code ID for Ardent Concord.
- Player 0 currently starts as Ardent Concord in the prototype scenario.
- Display names already present Ardent Concord instead of Tech.
- Ardent Concord worker: Keelwatch Ranker.
- Ardent Concord hero: Concord Marshal placeholder.
- Ardent Concord main building: Ledger House.
- Ardent Concord support building: Countersign Post.

Do not rename deterministic enum/schema IDs in pathfinder work unless a dedicated schema/codegen sprint is planned.

## Current Ardent Concord Identity In Repo

The current playable identity is intentionally simple:

- Balanced worker and building costs.
- High worker and hero durability compared with the first-pass opponents.
- Standard Countersign Post Holding value.
- Passive building mend: Ardent Concord owned main buildings and completed support buildings repair `5` HP every `60` simulation ticks while alive, damaged, and not under construction/deconstruction.
- Visual language in view primitives: compact, standardized forms with grey-blue hull metal and brass/orange accents.

These are enough to treat Ardent Concord as the first dependable baseline without locking final balance.

## Mission-Quality Priority Order

Polish Ardent Concord loops in this order:

1. Core command feel: selection, right-click movement, attack commands, gather/deposit, build placement, cancel/deconstruct, and repair commands.
2. Readability: faction silhouettes, selection rings, health labels, ownership states, construction state, repair state, and objective ownership.
3. Economy loop: Keelwatch Ranker production, Salvage/Plate/Holding costs, gathering cadence, dropoff clarity, support-building placement, and worker repair spending.
4. Combat loop: Concord Marshal survivability, attack range/damage readability, target feedback, death cleanup, and main-building defeat behavior.
5. Ardent sustain identity: passive mend presentation, repair-worker feedback, Countersign Post resilience, and later paired/countersignature hooks.
6. Objective loop: Quill capture, ownership buff, contested hold, and hold-to-win as an Ardent-led objective/tutorial target.
7. Mission framing: Ardent Concord Mission 1 / "Slack Water" scope once economy/construction/combat and Quill objective behavior are stable.

## First Mechanics To Make Production-Quality

Before broadening faction asymmetry, make these Ardent Concord behaviors feel reliable:

- Keelwatch Ranker selection, movement, gathering, building, and repair.
- Ledger House health, tier upgrade feedback, worker production, and defeat behavior.
- Countersign Post placement, validation, construction, Holding grant/removal, deconstruction, collision avoidance, and repairability.
- Concord Marshal selection, survivability, tech-scaling damage, death/rebuild loop, and combat feedback.
- Passive Ardent building mend, including clear HUD/world feedback when it matters.
- Quill objective ownership, resource trickle buff, contested-hold pause, and victory banner flow from an Ardent player perspective.

These priorities should be treated as gameplay and UX quality bars, not final balance claims.

## What To Avoid

- Do not rebalance Wrought or Gharn as part of Ardent pathfinder work unless a test is impossible without a narrow opponent adjustment.
- Do not add faction-specific hacks in Unity view code; gameplay behavior stays in Quantum simulation.
- Do not clone whole systems for Ardent Concord when a small faction/stat lookup or deterministic branch is enough.
- Do not rename `FactionId.Tech` or schema fields as incidental cleanup.
- Do not start Ardent Mission 1 implementation until the current ground loop is smoke-tested enough to support a scenario.

## Wrought And Gharn Role

Wrought and Gharn remain important:

- Wrought is the durability/network contrast opponent.
- Gharn is the aggressive/hold-ground contrast opponent.
- Both should keep enough functionality to test combat, economy pressure, Quill contests, and faction readability.
- Their prototype advantages should remain visible but rougher than Ardent Concord until the Ardent pathfinder loop is dependable.

## Future Hooks

Ardent Concord's deeper identity should be added later through explicit sprints:

- The Halving / The Fastening: paired units/buildings and Unheld state.
- Countersignature requirements for resupply, heal, reinforce, upgrade, or production boosts.
- Mission 1 / "Slack Water" objectives around proving mutual dependency under pressure.
- First air-domain probe, likely using the Rubbing-Kite, after `handoff/MOVEMENT_DOMAIN_QUANTUM_NOTE.md` is applied to live gameplay.

Each hook should start small, stay deterministic, and update this note if the meaning of pathfinder faction changes.
