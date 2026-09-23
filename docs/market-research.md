# Arrow Game: Name and Product Direction

Research date: 17 September 2026. This is a product recommendation, not a claim that the current build outperforms these games. Store listings describe what developers advertise; individual reviews are useful signals, not representative survey data. Competitors were researched through their listings, not installed and play-tested.

## Recommended Name

**ArrowNook: Logic Puzzle** (23 characters).

"Arrow" describes the mechanic, "Nook" suggests a small place to unwind, and "Logic Puzzle" explains the genre. Use **ArrowNook** as the short brand and `arrownook` as a possible repository name. This remains a proposal; the Unity product name and Android application ID have not been changed.

Alternative: **ArrowSprig: Logic Puzzle** (24 characters), for a more nature-inspired identity. Preliminary searches for these exact brands and an ArrowNook Google Play query found no obvious exact game-name match. Search coverage is incomplete; neither name is reserved or confirmed available.

Avoid Arrowloom and Arrowlume: both already have arrow puzzle listings. [Arrowloom: Threadway](https://play.google.com/store/apps/details?id=com.gametrail.arrowloom), [Arrowlume: Logic Puzzle](https://play.google.com/store/apps/details?id=com.blacksunset.arrowlume).

Google Play permits a title up to 30 characters, a short description up to 80, and a full description up to 4,000. Use accurate descriptions without ranking claims or repeated keywords. [Store listing fields](https://support.google.com/googleplay/android-developer/answer/9859152), [Metadata policy](https://support.google.com/googleplay/android-developer/answer/9898842).

Draft short description, subject to testing the release build:

> Find the clear path. Untangle arrow puzzles, one thoughtful tap at a time.

## What Competitors Already Offer

| Competitor | Advertised features | Opportunity suggested by the evidence |
| --- | --- | --- |
| [Arrows - Puzzle Escape, Lessmore](https://play.google.com/store/apps/details?id=com.ecffri.arrows) | Large level library, untimed play, hints, offline listing tag. | Visible reviews discuss intrusive ad frequency. Give players longer uninterrupted sessions. |
| [Arrow Puzzle, Easybrain](https://play.google.com/store/apps/details?id=com.easybrain.arrow.puzzle.game) | Daily challenges, trophies, hints, untimed puzzles, offline listing tag. | Visible reviews mention accidental taps and ad interruptions. Protect gestures and test small targets on real phones. |
| [Arrow Escape: Maze Puzzle Out](https://play.google.com/store/apps/details?id=com.arrow.puzzle) | Arrow escape puzzles with ads and purchases. | Visible reviews report stuck arrows, difficulty zooming, and an end-of-content blank screen. Validate movement, camera controls, and the campaign ending. |
| [Arrowlume: Logic Puzzle](https://play.google.com/store/apps/details?id=com.blacksunset.arrowlume) | Daily and Relax modes, hints, undo, automatic resume, themes, offline play, and an optional permanent unlock. | These conveniences are already competition, not unique selling points. Implement them well before claiming differentiation. |

Recommendation: compete on readable boards, dependable controls, useful assistance, and uninterrupted play. "More levels" or "daily puzzles" alone does not distinguish the game. Avoid promises of improved intelligence or memory; describe the puzzle experience itself.

## This Project's Starting Point

- Unity 6000.0.58f2, one main game scene, ten configured campaign levels.
- Existing five-heart mistake allowance, restart/settings screens, sound/haptics, and local level-number persistence.
- The level selector repeats the existing library after the authored levels; it does not produce new puzzles.
- No hint button before this pass; no board-state resume, undo, daily puzzle calendar, or gesture-based camera controls found in the inspected gameplay code.
- Product settings still identify the template as `SerapKeremGameKit` / `DefaultCompany`.

## First Implementation Pass

Implemented in source, pending Unity execution:

- A free, repeatable bulb hint button in the existing HUD. It checks the actual 2D colliders and highlights a stationary arrow with a clear forward path. Hints wait for movement to settle; a board without a clear exit returns an explicit status. This is a next-move check, not a full-board solvability proof.
- Single-pointer taps activate on release over the same arrow. Drags, multi-touch, UI interactions, canceled input, and locked gameplay cannot launch a new arrow.
- Moving arrows cannot be selected again. Stationary arrows do not spend lives because another arrow hits them.
- Arrow lifetime follows leaving the viewport, replacing the fixed three-second deletion timer. Tail motion clamps at bends instead of skipping past short segments.
- Removed duplicate empty-board notifications; retired levels deactivate before replacement; HUD life subscriptions do not accumulate on repeated display.

No ads, purchases, new level library, or store publication were added. The name has not been finalized. These improvements alone do not establish superiority over competitors.

## Prioritized Next Work

| Priority | Feature | Acceptance target |
| --- | --- | --- |
| P0 | Verify this pass on Android | All ten levels finish; blocked moves return correctly; no extra life loss on drags, modal taps, or repeated taps; no UI overlap on narrow/tall devices. |
| P0 | Pinch zoom, pan, and recenter | Players can inspect dense boards without launching an arrow; every edge stays reachable; controls respect device safe areas. |
| P0 | Resume the exact puzzle | Backgrounding or closing the app preserves the level, remaining arrows, and lives; completed levels are not replayed by accident. |
| P0 | Content validation and expansion | Every shipped board has a verified solution. Aim for 100+ varied boards for a first public release, with a clear campaign-end screen. This is a proposed target, not current content. |
| P1 | Undo and an optional relaxed mode | Reversing a mistake restores consistent board/life state; modes keep separate mastery records. |
| P1 | Daily puzzle and calendar | One stable board per date; replay does not duplicate rewards; progress works offline without punitive streak loss. |
| P1 | High-contrast and dark themes | Arrows and heads remain readable; a hint has a shape/motion cue as well as color; sound and haptics remain independent. |
| P2 | New puzzle mechanics | Prototype gates or direction switches only after the base game is tested. They should create planning choices, not unexplained failure. |

For monetization, the recommended starting design is no forced ads during a board, free retries, and free basic hints. Consider a clearly described one-time purchase for extra packs or cosmetics after testing whether players value them. This is a product proposal, not an implemented billing system or revenue forecast.

Measure a small Android test cohort before expanding: first-level completion, where players stop, accidental-tap reports, hint usage, crashes, and return play. Compare changes against the previous build rather than claiming an unmeasured advantage over another app.

## Before Store Submission

Finalize the chosen name, developer identity, and permanent Android package ID. Verify rights for the bundled third-party art/plugins separately from the source README's MIT statement, and keep required attribution. Prepare an app icon, screenshots of the actual build, support contact, and an accurate privacy/data-safety disclosure. Check current Play Console requirements when building the signed Android App Bundle.

The project has not been built or uploaded to Google Play. Unity is not installed in the current workspace environment; see [verification.md](verification.md) for the editor and device checks still required.
