# Kilnfall Prototype Smoke Test Checklist

Use this checklist after each small sprint. Keep the run short, and record only pass/fail plus any visible regression.

## Setup

- Open the prototype scene in Unity.
- Enter Play mode.
- Confirm the HUD appears and the game advances frames.
- Confirm Player 0 displays as Ardent Concord.
- Confirm no new Unity console errors appear during startup.

## Selection

- Select one Player 0 worker.
- Confirm the selected unit highlights.
- Confirm the HUD shows the unit as a Keelwatch Ranker.
- Select the Player 0 hero.
- Confirm the HUD shows the hero as Concord Marshal.
- Select the Player 0 main building.
- Confirm the HUD shows Ledger House.

## Movement

- Select a Player 0 worker.
- Right-click an open ground position.
- Confirm the worker moves toward the clicked location.
- Confirm no other selected state or HUD panel breaks during movement.

## Gathering And Deposit

- Select a Player 0 worker.
- Right-click a Salvage node.
- Confirm the worker gathers and carries resources.
- Wait for the worker to deposit at the main building.
- Confirm Player 0 Salvage increases in the HUD.
- Repeat once with Plate if a Plate node is available nearby.

## Worker Production

- Select the Player 0 Ledger House.
- Press `B`.
- Confirm a worker is queued or produced.
- Confirm the new worker is selectable.
- Confirm the new worker can receive a move command.

## Support Placement And Construction

- Select a Player 0 worker.
- Move the cursor near the worker and press `C`.
- Confirm the placement preview appears as a low foundation footprint.
- Move the cursor between valid and invalid locations.
- Confirm valid and invalid preview colors still update.
- Place the support building in a valid location.
- Confirm the worker begins building.
- Confirm the foundation uses the Ardent Concord construction visual state.
- Wait for completion.
- Confirm the building becomes Countersign Post and Holding increases.

## Cancel, Refund, And Deconstruction

- Start a new Countersign Post foundation.
- Select the unfinished foundation or assigned builder and press `X`.
- Confirm the foundation cancels and resources refund.
- Select a completed Countersign Post and press `X`.
- Confirm the building enters deconstruction state.
- Confirm the deconstruction visual state differs from both foundation and completed building.
- Wait for deconstruction completion.
- Confirm the building is removed and Holding decreases.

## Combat And Defeat

- Select a Player 0 combat-capable unit or hero.
- Right-click an enemy unit or building.
- Confirm the attacker approaches and attacks.
- Confirm health bars update.
- Confirm defeated units are cleaned up or visibly dead.
- If time allows, destroy an enemy main building.
- Confirm defeat state still triggers and the HUD remains readable.

## Kilnfall Presentation Check

- Confirm visible labels use Kilnfall names, not prototype faction names.
- Confirm resource labels display Salvage, Plate, and Holding.
- Confirm Ardent Concord, Wrought, and Gharn colors remain visually distinct.
- Confirm construction and deconstruction states are readable at the current camera zoom.
- Note any HUD clipping or text overlap.

## Result Log Template

Sprint tested:

Date:

Result: pass / pass with notes / fail

Notes:

- 
