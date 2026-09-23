# Gameplay Verification

The code and prefab changes in this pass still require Unity 6000.0.58f2. No Unity editor executable was available in the current environment, so editor compilation, regression execution, visual layout, and Android runtime behavior are **not yet verified**.

Checks completed locally: `git diff --check`; YAML parsing and local-reference validation for all 62 HUD prefab objects; resolution of 13 referenced asset/script GUIDs; confirmation that the game UI references the edited HUD. The suggested title is 23 characters and the draft short description is 74 characters. These are static checks, not a Unity build.

## Automated Editor Checks

Open the project, allow imports to complete, and use **Tools > Arrow Game > Run Regression Checks**. The checks run in an isolated temporary scene and clean up their fixtures. Run outside Play mode. A thrown exception marks a failure; a success message appears only after all checks pass.

For an installed, licensed Unity editor, the same checks can be run in batch mode:

```sh
rtk proxy /Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -nographics -quit -projectPath '/Users/anupampradhan/Documents/Arrow Game' -executeMethod _Game.Editor.ArrowGameRegressionChecks.Run -logFile /private/tmp/arrow-regression.log
```

Checks cover input cancellation, hint obstruction including trigger colliders, rejection of hints during motion, moving-arrow click rejection, low-frame-rate movement/completion, collision reversal, duplicate line-removal notifications, and HUD prefab references. These checks do not replace device testing or prove every board solvable.

## Device Acceptance

1. Finish all ten authored levels and test restart/next repeatedly. During replacement, old colliders must not remain selectable. Verify the existing repeat behavior after the last level; a dedicated campaign ending is still future work.
2. Tap Hint on an idle board. Exactly one arrow with a clear forward path highlights. No coins, life, or ad is required. While arrows move, Hint is disabled. A blocked board reports "No clear exit".
3. Tap a blocked arrow and allow it to return. Only the moving arrow causes a life loss; it becomes usable again after returning. Repeat at simulated 15, 30, and 60 fps and on a low-end Android device.
4. Tap and release the same arrow: one launch. Drag away then return: no launch. Press one arrow and release another: no launch. Touch with two fingers: no launch, even when one finger lifts first.
5. Interact with restart, settings, and result screens over the board. No arrow beneath the UI launches. Background the app while pressing, resume, and release: no launch. Exact board-state saving across process termination is not implemented yet.
6. Test 360x640, 412x915, and tablet-sized portrait viewports plus a phone with gesture navigation/cutout. Check the hint icon/status, hearts, and board edges for clipping or overlap. The existing camera fitting is not safe-area-aware.
7. Disable network connectivity and solve several levels. Confirm hints, sound toggles, restart, and level-number persistence work locally. Do not advertise full offline support until the Android build passes.
8. Check a moving arrow that takes longer than three seconds to exit. It must stay present until it leaves the viewport and must not cause an early win.

No automated Android build, screenshot validation, performance result, or competitive playtest has been completed in this environment.
