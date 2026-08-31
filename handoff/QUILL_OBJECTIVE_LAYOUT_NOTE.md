# Quill Objective Layout Note

Created: 2026-08-31

Purpose: keep Quill objectives valuable without letting a starting spawn accidentally trigger a match loss.

## Current Correction

The active prototype Quill objective is now centered at `(0, 0)`.

The Quill no longer directly defeats players through a hold timer. Owning it keeps the resource trickle benefit, but a player still needs to defeat opponents through normal base destruction.

## Updated Start Distance Check

After the eight-faction map expansion, with the Quill still at `(0, 0)`, approximate main-base distances are:

- P0 Ardent Concord at `(0, -55)`: 55 units.
- P1 Wrought at `(-40, -40)`: about 57 units.
- P2 Gharn at `(40, -40)`: about 57 units.
- P3 Seethe at `(-58, 0)`: 58 units.
- P4 Veirn at `(58, 0)`: 58 units.
- P5 Vaelun at `(-40, 40)`: about 57 units.
- P6 Nimhara at `(40, 40)`: about 57 units.
- P7 Virii at `(0, 55)`: 55 units.

Each faction now has a local Salvage/Plate pair near its start. The previous P7 proximity problem is removed.

Support-building placement now allows a 68-unit half-extent, leaving growth room inside the visible 72-unit boundary.

## Future Multi-Quill Direction

A later map sprint can add multiple Quill towers, likely five, spread across a larger map. Holding all towers can become a possible win-pressure condition, but it should not immediately defeat opponents.

Preferred future rule:

- Holding one Quill grants local/economy benefit.
- Holding several Quills increases pressure.
- Holding all Quills starts damaging enemy main bases over time.
- Enemy players can stop the damage by recapturing at least one Quill.
- Main-base destruction remains the actual defeat condition.

## Map Expansion Direction

The eight-faction bootstrap now has enough first-pass room for local growth, but terrain remains view-only.

Good next terrain work:

- Enlarge the playable ground bounds beyond the current square.
- Move all eight starts into an intentional ring or opposed-cluster layout.
- Keep every start outside the Quill capture radius plus a safety buffer.
- Add lakes and rivers as visual/navigation identity pieces before they become domain gameplay.
- Add mountain/shard obstacles as pathing tests only after ground pathing remains stable.
- Keep resource pairs readable and avoid placing them inside Quill capture radius unless that is the explicit contest point.
