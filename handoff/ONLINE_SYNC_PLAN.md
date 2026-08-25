# Online Sync Plan

Created: 2026-08-23.

Purpose: keep this local Photon Quantum prototype aligned with The First Hand / online project board without exposing local machine paths, credentials, or noisy implementation details.

## Sync Principle

Use the local repo as the implementation source of truth and The First Hand / online board as the product-planning source of truth.

Do not paste raw local handoff files directly into the site when they contain absolute paths, private environment details, or low-level implementation notes. Instead, publish a cleaned export snapshot.

## Recommended Flow

### Local To Online

1. Finish one sprint locally.
2. Verify it manually or via compile/smoke test when available.
3. Update:
   - `handoff/PROJECT_STATE.md`
   - `handoff/KILNFALL_SPRINT_PLAN.md`
4. Add or refresh `handoff/ONLINE_STATUS_EXPORT.md`.
5. Paste/upload the export into the online project board or First Hand Codex.

### Online To Local

1. Review new First Hand/project-board entries.
2. Summarize the source into local handoff notes.
3. Update the sprint plan only when online material changes order, scope, or gates.
4. Do not overwrite local implementation docs with online prose unless the user explicitly asks.

## What To Sync

Sync these items after each sprint:

- Sprint number and title.
- Status: complete, in progress, blocked, or not started.
- One-sentence result.
- Verification status.
- Next recommended sprint.
- Any source-guidance changes from First Hand.

Avoid syncing:

- Absolute paths like `/Users/...`.
- Passwords, cookies, tokens, or account/session details.
- Raw compile logs unless a short error summary is needed.
- Internal tool chatter.
- Long implementation diffs.

## Online Export Shape

The online export should have these sections:

- Current Build Snapshot
- Completed Sprint Log
- Active/Next Sprint
- First Hand Alignment Notes
- Blockers / Verification Notes
- Clean Next Actions

## Automation Candidate

Later, add a small script that reads `handoff/KILNFALL_SPRINT_PLAN.md` and `handoff/PROJECT_STATE.md`, then writes `handoff/ONLINE_STATUS_EXPORT.md` automatically.

Keep that script read-only over source docs and write-only to the generated export file.

## First Manual Export

The first manual export is `handoff/ONLINE_STATUS_EXPORT.md`.
