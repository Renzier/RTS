# First Hand Review Notes

Reviewed: 2026-08-23 via signed-in Chrome session.

Source: https://thefirsthand.kilnfall.com/

Access: authenticated, internal/spoiler-full. Do not store credentials, cookies, tokens, passwords, or session data in this repository.

## What This Source Is

The First Hand is the private working codex for Kilnfall. It contains canon, game design, wiki/index material, concept art, audio references, progress records, and studio/internal operational material. The site separates spoiler-free and spoiler-full/internal views behind sign-in, and pages identify their spoiler tier and source file.

Major sections observed:

- Explainers: newcomer guide and deep-dives.
- Lore: world bible, setting, peoples, campaigns, canon.
- Codex: game design record and implementation direction.
- Wiki: glossary and cross-linked index.
- Gallery: concept art plates.
- Audio: music, dialogue, and sound references.
- Progress: project state/session records.
- The Studio: internal Elo Studios operations.

## Build-Relevant Direction

The internal Game Design Document identifies Kilnfall, working title Project Vanguard, as a competitive RTS for Windows built in Unity 6 DOTS. Its target feel combines hero/ability RPG texture, macro precision, cinematic scale, and chunky unit readability.

The four domains are ground, air, space/orbit, and underwater. These should be built from one entity model, not as four unrelated game engines. The internal architecture uses MovementDomain as the organizing idea: shared entity identity with domain-specific movement, targeting, camera, detection, and production routing.

The key unifying mechanics are the Grain/Cast/Seal model and the Virii copy rule: the Virii can copy recorded form, but not true belonging. Future systems should treat Grain-loud state, Seal/Tell checks, faction identity, repair/heal/transform exposure, and Virii copying as generic data contracts.

The internal roadmap strongly supports a ground-first sequence. Ground is the core vertical slice and should include air where practical. Space/orbit and underwater are real pillars, but should come after the ground loop proves fun and deterministic foundations are solid. Ranked 1v1 launch scope is described as Ground(+Air), with space and underwater promoted later after determinism risk is controlled.

## Faction And Campaign Direction

The GDD confirms eight playable civilizations and a three-way ideological fracture across each faction: Claimants, Quenchers, and Wardens. Tech trees, hero branches, and campaign routes should eventually express those same three directions.

Ardent Concord is called out as the pathfinder faction for early implementation. The vertical-slice plan recommends an Ardent Concord Mission 1 target, "Slack Water," as the first real mission once economy/combat are stable.

The current public-site faction mappings remain valid for the prototype:

- Tech -> Ardent Concord
- Fantasy -> Wrought
- Hybrid -> Gharn

Continue using the more specific faction names, buildings, and mechanics already recorded in `handoff/KILNFALL_GAME_DIRECTION.md`.

## Roadmap Implications For This Repo

This Unity + Photon Quantum prototype is not the same architecture as the internal Unity DOTS Project Vanguard docs. Do not blindly port DOTS-specific instructions into this repo. Use First Hand as product direction while preserving the local architecture rule: gameplay decisions stay in Quantum simulation, and Unity view code only displays state or submits input.

Practical implications:

- Continue small sprint steps rather than attempting a large architecture rewrite.
- Prioritize the ground RTS loop already present: selection, movement, economy, production, construction, combat, and main-objective flow.
- Keep the early faction focus on Ardent Concord, Wrought, and Gharn.
- Treat Ardent Concord as the primary pathfinder faction for first mission-quality work.
- Add an architecture note before implementing domain expansion so MovementDomain-style thinking can be adapted to Photon Quantum safely.
- Add Grain-loud, Seal/Tell, and Virii hooks as reusable data contracts later, not bespoke per-faction special cases.
- Delay serious space/orbit and underwater gameplay until ground construction/pathing/economy/combat are stable.

## Sprint Plan Adjustments Recommended

Keep Sprints 1-8 as-is because they are low-risk presentation conversion and do not conflict with First Hand.

Before domain expansion sprints, add or prioritize a MovementDomain architecture note adapted to Photon Quantum.

Before Virii implementation, add or prioritize a Grain/Cast/Seal data-contract sprint so the copy mechanic has a shared foundation.

Before campaign sprinting, add Ardent Concord Mission 1 / Slack Water as the first mission target, but only after the current economy/construction/combat loop is stable.

## Verification Notes

- Home page and Codex index were successfully read from the signed-in Chrome tab.
- Game Design Document, Build Roadmap, and Vertical Slice Plan pages were successfully reviewed.
- Direct navigation to one Progress detail page was blocked by the browser, but the Progress index was readable.
