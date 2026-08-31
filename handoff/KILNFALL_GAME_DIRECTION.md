# Kilnfall Game Direction Reference

Source reviewed: https://kilnfall.com/ on 2026-08-23.

Additional source reviewed: https://thefirsthand.kilnfall.com/ on 2026-08-23 via authenticated Chrome session. See `handoff/FIRST_HAND_REVIEW_NOTES.md`.

This document redirects the current Anachron RTS prototype toward the Kilnfall world. Treat it as a production-facing reference for faction naming, world rules, environment language, building direction, and future implementation choices.

## Direction Shift

The prototype should stop presenting itself as a generic Tech/Fantasy/Hybrid RTS. Kilnfall is a story-driven, four-domain RTS set in a dying-star world where armies fight across ground, air, ocean, and orbit. The main strategic question is not simply defeating enemy bases; it is who controls the Quills when the Kiln performs the Last Firing and rewrites reality.

Near-term implementation should preserve the existing deterministic RTS foundation while renaming and reframing systems around Kilnfall:

- Replace Tech/Fantasy/Hybrid with early Kilnfall powers.
- Replace normal terrain/map framing with Shards, Mere, Quill islands, industrial flues, abyssal roots, and orbital crown wreckage.
- Treat main bases as faction-specific Quill footholds or campaign staging structures.
- Treat supply buildings as faction-specific holding, signing, oath, kiln, debt, appetite, tide, or printing infrastructure.
- Keep the current ground RTS prototype, but design every new system so it can eventually connect to air, underwater, and orbit.

## World Pillars

### The Kiln

The Kiln is the dead or dying star at the center/bottom of the setting. It no longer provides warmth; it provides pressure, gravity, and reality-writing authority. It is the engine behind the world's rules.

Design implications:

- The world should feel compressed, red-lit, old, pressurized, and unstable.
- Heat is scarce, political, and faction-defining.
- Endgame pressure should be existential: the world is running out of time, not just resources.

### The Mere

The Mere is a vast ocean wrapped around the Kiln. It is hundreds of kilometers deep, reaches outward into space, and is slowly boiling away.

Design implications:

- Water is not background. It is the main world-body.
- Maps should feel like islands, platforms, shipyards, reefs, floating remnants, and structures rising from impossible depth.
- Shorelines, tides, submerged roots, docks, and abyssal approaches should matter mechanically over time.

### The Shards

The Shards are broken pieces of an older world floating in and on the Mere, carrying forests, cities, wreckage, and ruins.

Design implications:

- Ground maps should be broken, irregular, layered, and surrounded by water, abyss, or machinery.
- Resource nodes can be reframed as salvage, sealed records, heat stores, oath-furnaces, tidewood, or Grain-rich wreckage.
- Neutral sites can be old-city fragments, wrecked fleets, root-flange ruins, or abandoned Quill machinery.

### The Twelve Quills

The Quills are ancient world-spanning towers. Each runs from ocean floor to island waist, through the sky, and into orbit. Twelve exist; only four are lit and fully relevant to the Last Firing.

Design implications:

- A Quill is not a normal capture point. Full control should require holding linked parts in multiple domains.
- Current ground-only prototype can represent the Quill waist as a first step.
- Future domain expansion should add root, spire, and orbital anchor control.
- Losing the last Quill should eventually mean loss of command authority, not merely loss of territory.

### The Last Firing

When the Kiln finally fails, it writes one final world from the pattern loaded into the lit Quills. Only one author can define the next world.

Design implications:

- Campaign conquest should revolve around authorship, not extermination.
- Faction victory screens and mission objectives should ask what kind of world gets written.
- Every faction's mechanics should express how it proves identity, claim, continuity, or authority.

### The Grain

The Grain is the reality-record beneath matter. Healing, repair, transformation, copying, and identity all expose or alter the Grain. This is why counterfeit copies, faction tells, signatures, oaths, and records matter.

Design implications:

- Repair/healing should be powerful but risky.
- Identity checks should become a future anti-infiltration layer.
- UI language should lean on records, signs, ledgers, signatures, readings, oaths, patterns, and copies.

## Four Domains

### Ground

Current prototype domain. Ground should focus on Quill waist-islands, Shards, settlements, shipyards, reefs, industrial platforms, forests, and battlefield salvage.

Prototype guidance:

- Convert current flat RTS terrain into a Quill-adjacent island/shard map.
- Use water, cliffs, docks, ribbed shipyards, and root machinery as the primary map identity.
- Keep workers, bases, resources, combat, and construction in this domain for now.

### Air

The breathable sky is called the Skin. Above it is the Thinning, where conventional flyers stall or fail.

Future guidance:

- Air should not behave like generic aircraft. Each faction's relationship to altitude differs.
- Air routes can bridge Shards, raid supply lines, scout Quill spires, or attempt risky transitions toward orbit.

### Ocean / Underwater

The Mere is a deep battlefield with crushing pressure, sonar/Grain-noise, hidden routes, and Quill roots.

Future guidance:

- Underwater should be the stealth, pressure, root-control, and surprise-attack domain.
- Quill root control belongs here.
- Noise/signature systems should matter more underwater than on land.

### Orbit

The Crown Shallows are near-orbit junkyards of broken rock, wreckage, fleet paths, and open darkness.

Future guidance:

- Orbit should provide Quill anchor control, capital ships, strategic arrival routes, and command disruption.
- Wrought and Vaelun should feel especially strong here.
- Losing orbital control of a Quill can eventually disrupt command below.

## Factions / Powers

There are eight powers. The current prototype supports three faction IDs, so the recommended first conversion is:

1. Tech -> Ardent Concord
2. Fantasy -> Wrought
3. Hybrid -> Gharn

Add the remaining five once the three-faction prototype is stable.

### Ardent Concord

Core identity: human, legalistic, repair-oriented, reliable across all domains, built around mutual countersignature.

Theme line: nobody is valid alone.

Signature mechanic: The Halving / The Fastening. Units and buildings are paired with a counterpart. If the counterpart is destroyed, the paired asset becomes Unheld: still active, but unable to resupply, heal, reinforce, or upgrade until restored or lost.

Prototype translation:

- Worker: Keelwatch Ranker support crew or dockhand.
- Hero: Marshal Yesa Corrun or Keel-Captain Ovid Rhennick later; for now use a Concord Marshal.
- Main base: Ledger House / Quill Holding Office / Keelwatch Command.
- Supply building: Countersign Post, Tally Yard, or Fastening Office.
- Visual language: brass tally-bars, stamped plates, grey-blue hull metal, lifeboat orange, worn canvas, standardized hatches.
- Gameplay start: balanced stats, repair bonuses, supply resilience, paired-building vulnerability later.

Known force references:

- Keelwatch Ranker: line infantry.
- Rubbing-Kite: air reconnaissance.
- Hush-class Sounder: underwater vessel.
- Common Gauge Yard: colossal shipyard.

### Wrought

Core identity: machine maintenance civilization, legalistic persistence, unbroken network, order without relief.

Theme line: maintain until relieved; relief never came.

Signature mechanic: The Longhold. Serious actions require network agreement or countersignature. Their Count only rises and proves authentic connection. Wreckage, cover, repair, and held ground feed momentum.

Prototype translation:

- Worker: Wright.
- Hero: Grist, Serial One or Kerf later; for now use a Wrought Overseer.
- Main base: Longhold Node / Maintenance Court / Crownfast Yard.
- Supply building: Count Relay, Standing Node, or Plate Printer.
- Visual language: machine-plate, riveted modular cover, stamped serials, printed barricades, dull industrial lights.
- Gameplay start: stronger buildings, slower but tougher economy, cover/repair identity later.

Known force references:

- Wright: worker.
- Anchor Driver: siege engine.
- Ashlar-class Crownfast: space capital.
- Crownyard Slipways: megastructure.

### Gharn

Core identity: oath-bound people who rewrote themselves through pain, honor made physical, strongest on the ground.

Theme line: every kept oath hardens the army; every broken oath has a cost.

Signature mechanic: Oathpyre / Tally / Weight. The Gharn swear achievable objectives. Kept oaths grant permanent army strength; broken oaths halve accumulated bonuses and create Slag. Lines get harder to move the longer they hold, but lose the built Weight if they step back.

Prototype translation:

- Worker: Sinterjack laborer or oath-bearer.
- Hero: Sekha Ashcollar or Ghaddo Twopunch later; for now use a Tally Captain.
- Main base: Oathpyre.
- Supply building: Tally Stone, Ashcollar Furnace, or Weight Marker.
- Visual language: branded flesh, furnace scars, black iron, ash, throat marks, oath circles.
- Gameplay start: high ground combat strength, weaker economy/air tech, bonuses for holding position later.

Known force references:

- Sinterjack: line infantry.
- Ashfall Riders: air cavalry.
- Lotsworn: underwater heavy.
- Brakework Hulls: space heavy.

### Seethe

Core identity: civic dragoncraft; human/dragon people using Reading Kilns and battlefield pattern archives.

Theme line: the kiln offers answers; the reader decides.

Signature mechanic: Working Set / Re-Read. Seethe collect battlefield patterns into a limited set, license answers, and recommission draconic forms at field Reading Kilns. Over-specialization reduces flexibility.

Prototype translation:

- Main base: Reading Kiln.
- Supply building: Loam Record, License Kiln, or Pattern Archive.
- Visual language: draconic forms, loam, archive leaves, civic seals, biological record machinery.

Known force references:

- Harrowmouth: commissioned line form.
- Boardback: ground heavy.
- Fathomjaw: underwater leviathan.
- The Incipit: field Reading Kiln.

### Veirn

Core identity: infernal debt economy; living star-fire from beneath the Mere.

Theme line: build now, pay later.

Signature mechanic: Ordal / Keth / Tally. Units can be paid for slowly and honestly or issued through debt. Being Banked makes the army cold and hidden; being Owed makes it fast but visible. Overspending can trigger foreclosure.

Prototype translation:

- Main base: Ledger Furnace.
- Supply building: Keth House, Debt Chimney, Slagbound Furnace.
- Visual language: caul-glass, ember cores, dark heat, accounting marks, red-black glow.

Known force references:

- Cauled: line infantry.
- Mere-Drake: underwater hunter.
- Brightlance: air assault.
- Hearthbarge: orbital capital/logistics.

### Vaelun

Core identity: living energy inside black hunger armor; local energy reserve and appetite management.

Theme line: every hunger has a name and a limit.

Signature mechanic: Appetite / Overfeed / Want. Units spend local appetite to act and must feed from tenders, machinery, radiation, or captured incoming energy. Opening armor too far risks the person inside dispersing.

Prototype translation:

- Main base: Ration Vault.
- Supply building: Appetite Tender, Intake Dock, or Blackwake Vault.
- Visual language: black armor, ration cells, negative heat, shutters, heavy service lines.

Known force references:

- Hollowguard: line interceptor.
- Tithe-Hound: machine hunter.
- Nightshear: committed assault.
- Blackwake: migration ironclad.

### Nimhara

Core identity: tidal civilization, oldest people in the world, spending their remaining future through tide magic.

Theme line: borrow from the tide, pay in years.

Signature mechanic: The Draw / Falls. Power borrowed on the rising tide is free; on the falling tide it costs from the civilization's finite remaining lifespan. This remaining life never refills.

Prototype translation:

- Main base: Tidewood Grove.
- Supply building: Tally-Bough, Sill Grove, Tide Marker.
- Visual language: waterline trees, pale bands, tidewood, moon-fragment pull, quiet skiffs.

Known force references:

- Sillwright: line specialist.
- Tide-Skiff: surface vessel.
- Struck Lance: air heavy.
- Nine-Note Trebuchet: siege.

### The Virii

Core identity: copying/infiltration power that cannot touch whole identities, only exposed records.

Theme line: they take the Cast, never the Seal.

Signature mechanic: Rubbing / Sounding / Take / Fold / Platen. The Virii watch for units that heal, repair, transform, promote, resurrect, or channel power, then copy those Grain-loud patterns. Copies look right but lack true belonging and fail faction-specific tells.

Prototype translation:

- Main base: The Fold.
- Supply building: Platen, Sounding Node, Skinline Updraft Node.
- Visual language: ash/silt/wreckage bodies, wrong seams, blank identity, copied silhouettes, silent gaps.

Current repo status:

- Virii are assignable as the eighth prototype faction.
- `FactionId.Virii = 7`.
- Future Virii implementation should build on `handoff/GRAIN_CAST_SEAL_CONTRACT.md`.
- First hooks should observe `GrainState`, copy Cast through a shared path, and fail Seal/Tell checks that require true belonging.
- No Rubbing, Sounding, Take, Fold, Platen, copying, or infiltration mechanic is implemented yet.
- Placeholder names to preserve for later: The Fold, Platen Node, Sounding Node, Skinline Updraft Node, A Draft, Kin-shape.

Known force references:

- A Draft: base form.
- Kin-shape: stolen infantry.
- The Fold: printing structure.
- Skinline Updraft Node: air transit structure.

## Buildings Direction

Current building types are Main Building and Supply Building. Rename and reskin these by faction first, then add mechanics.

### Main Building Equivalents

- Ardent Concord: Ledger House, Keelwatch Command, Quill Holding Office.
- Wrought: Longhold Node, Maintenance Court, Crownfast Yard.
- Gharn: Oathpyre, Tally Hearth, Ashcollar Furnace.
- Seethe: Reading Kiln, Incipit, Loam Archive.
- Veirn: Ledger Furnace, Keth Office, Heat Account.
- Vaelun: Ration Vault, Blackwake Dock, Appetite Core.
- Nimhara: Tidewood Grove, Tally-Bough Grove, Sill Court.
- Virii: The Fold, Fair Hand Archive, Platen House.

### Supply Building Equivalents

- Ardent Concord: Countersign Post. Adds food/supply by expanding valid signatory capacity.
- Wrought: Count Relay. Adds supply by extending the Longhold network.
- Gharn: Tally Stone. Adds supply by recording oaths that the army can carry.
- Seethe: Pattern Stack. Adds supply by expanding licensed working records.
- Veirn: Keth Ledger. Adds supply by raising debt capacity.
- Vaelun: Appetite Tender. Adds supply by storing rationed local energy.
- Nimhara: Tide Marker. Adds supply by expanding grove pull, possibly at future lifespan risk.
- Virii: Platen Node. Adds supply by increasing copy throughput or stable bodies.

### Neutral / Map Structures

- Quill waist-island: central strategic objective; current prototype can use this as a capture/defense landmark.
- Root-flange: future underwater objective.
- Spire elevator/chimney: future air transition objective.
- Orbital anchor: future orbit objective.
- Common Gauge Yard / Graving Dock: shipyard or heavy production site.
- Lit Chimney: heat/Veirn route, strong visual landmark.
- Vellum Hold: layered settlement, bridge/stairs environment.
- Sablehearth Flue-Hall: interior furnace/industrial mission space.
- Uhl Kethaun, the Owing Mouth: abyssal gate or boss-scale mission landmark.
- Ammeloth, the Anchored Grove: Nimhara living island.

## Prototype Renaming Pass

Recommended first code-facing pass:

- `Tech` display name -> `Ardent Concord`
- `Fantasy` display name -> `Wrought`
- `Hybrid` display name -> `Gharn`
- Worker label -> faction-specific worker label where possible.
- Hero label -> faction-specific placeholder hero label.
- Main Building label -> faction-specific main building name.
- Supply Building label -> faction-specific supply building name.
- Resource labels:
  - Wood -> Salvage
  - Iron -> Heat-Iron or Plate
  - Food/Supply -> Holding / Capacity / Muster, depending UI scope

Do not rename deterministic enum IDs unless the team is ready for a wider schema/codegen pass. Display names can shift first.

## Visual Direction

Avoid generic fantasy/tech silhouettes. Every asset should look like it belongs to a wet, red-lit, pressure-bound, record-obsessed world.

Global palette and environment cues:

- Dull ember red from the Kiln.
- Storm-dark ocean surfaces.
- Pale abyssal growth and bone/metal Quill structures.
- Blackened vaults, ribbed shipyards, old wreckage, wet stone.
- Upward rain, exposed roots, huge cables, flues, pressure vessels.
- Layered settlements built onto impossible vertical structures.

Faction silhouette cues:

- Ardent Concord: practical hulls, brass marks, standardized human kit.
- Wrought: blocky machine bodies, serial plates, modular printed cover.
- Gharn: heavy grounded bodies, scars/brands, furnace implements.
- Seethe: draconic civic forms, loam, archive-kiln motifs.
- Veirn: ember bodies, glass cauls, debt-ledger ornaments.
- Vaelun: black armor, ration hardware, shutters, heavy vault mass.
- Nimhara: tidewood, pale tally marks, skiffs, waterline ritual forms.
- Virii: copied silhouettes with wrong seams, ash/silt/wreckage bodies.

## Gameplay Roadmap Implications

### Immediate

- Update UI names and primitive faction visuals to the first three Kilnfall powers.
- Rename supply/main building display strings by faction.
- Reframe the map as a Quill waist-island or Shard battlefield.
- Adjust handoff docs so future tasks know Anachron prototype is now a Kilnfall prototype.

### Next

- Add a Quill objective or neutral central structure.
- Add faction-specific supply building stats/effects.
- Add first-pass faction mechanics:
  - Concord: repair/supply reliability.
  - Wrought: stronger buildings or deployable cover.
  - Gharn: hold-position combat bonus.

### Later

- Add air layer as map-to-map or lane-based support before full 3D domain control.
- Add underwater root-control missions.
- Add orbital anchor control.
- Add Grain-loud states triggered by healing, repair, upgrade, and hero rebuild.
- Add Virii copying/infiltration once repair/heal hooks are mature.

## Canon Guardrails

- Do not make the world a normal planet.
- Do not reduce the Quills to ordinary towers or flags.
- Do not make factions symmetrical with different skins.
- Do not treat resources as generic wood/gold long term.
- Do not make victory only about destroying bases; base defeat is prototype scaffolding, while Kilnfall victory is authorship, control, and survival through the Last Firing.
- Keep story and mechanics coupled. In Kilnfall, a faction's economy is also its law, faith, biology, or identity system.
