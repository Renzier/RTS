# Quill Objective Layout Note

Created: 2026-08-31

Purpose: keep Quill objectives valuable without letting a starting spawn accidentally trigger a match loss.

## Current Correction

The active prototype Quill objective is now centered at `(0, 0)`.

The Quill no longer directly defeats players through a hold timer. Owning it keeps the resource trickle benefit, but a player still needs to defeat opponents through normal base destruction.

## Start Distance Check

With the Quill at `(0, 0)`, approximate main-base distances are:

- P0 Ardent Concord at `(0, -13)`: 13 units.
- P1 Wrought at `(-17, 14)`: 22 units.
- P2 Gharn at `(17, 14)`: 22 units.
- P3 Seethe at `(0, 31)`: 31 units.
- P4 Veirn at `(-29, -1)`: 29 units.
- P5 Vaelun at `(29, -1)`: 29 units.
- P6 Nimhara at `(0, -33)`: 33 units.
- P7 Virii at `(-6, 5)`: 8 units.

P7 remains the closest because the eight-faction bootstrap was added around an older, smaller test layout. That should be treated as a map-layout follow-up, not a victory-rule shortcut.

## Future Multi-Quill Direction

A later map sprint can add multiple Quill towers, likely five, spread across a larger map. Holding all towers can become a possible win-pressure condition, but it should not immediately defeat opponents.

Preferred future rule:

- Holding one Quill grants local/economy benefit.
- Holding several Quills increases pressure.
- Holding all Quills starts damaging enemy main bases over time.
- Enemy players can stop the damage by recapturing at least one Quill.
- Main-base destruction remains the actual defeat condition.

## Map Expansion Direction

The current eight-faction bootstrap needs more breathing room before objective win pressure returns.

Good next terrain work:

- Enlarge the playable ground bounds beyond the current square.
- Move all eight starts into an intentional ring or opposed-cluster layout.
- Keep every start outside the Quill capture radius plus a safety buffer.
- Add lakes and rivers as visual/navigation identity pieces before they become domain gameplay.
- Add mountain/shard obstacles as pathing tests only after ground pathing remains stable.
- Keep resource pairs readable and avoid placing them inside Quill capture radius unless that is the explicit contest point.
