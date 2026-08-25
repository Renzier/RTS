# Kilnfall Source Registry

Created: 2026-08-23.

Use this file to track external Kilnfall references used to direct the game. Do not store passwords, session tokens, cookies, or private credentials in this repository.

## Sources

### Public Site

- URL: https://kilnfall.com/
- Access: public
- Reviewed: 2026-08-23
- Current use: primary world, faction, building, unit, Quill, Grain, and Last Firing direction source for `handoff/KILNFALL_GAME_DIRECTION.md`.

### The First Hand

- URL: https://thefirsthand.kilnfall.com/
- Access: authenticated
- Account identifier supplied by user: `doug@eloai.co`
- Password: not stored; request only if an active authenticated review is needed.
- Added: 2026-08-23
- Reviewed: 2026-08-23 via signed-in Chrome session
- Current use: internal/spoiler-full source for high-level build direction, project architecture direction, domain sequencing, and roadmap refinement. See `handoff/FIRST_HAND_REVIEW_NOTES.md`.

## Access Notes

- If authenticated content is needed, prefer having the user sign in through the browser/session rather than writing credentials into files.
- If the user supplies a password in chat, use it only for the immediate authenticated review and do not save it.
- When content from an authenticated source changes design direction, update `handoff/KILNFALL_GAME_DIRECTION.md`, `handoff/KILNFALL_SPRINT_PLAN.md`, and `handoff/PROJECT_STATE.md` with a short source note.
