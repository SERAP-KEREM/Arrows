# ArrowNook UI Theme — Unity Inspector Setup

> Do this after opening Unity and letting it compile the new scripts.

## 1. Download & Import Nunito Font

1. Go to [fonts.google.com/specimen/Nunito](https://fonts.google.com/specimen/Nunito)
2. Download **Nunito Bold** and **Nunito Regular** (`.ttf` files).
3. In Unity: drag both `.ttf` files into `Assets/_Game/Fonts/` (create the folder).
4. In the Project window, right-click each `.ttf` → **Create → TextMeshPro → Font Asset**.
5. Name them `Nunito-Bold SDF` and `Nunito-Regular SDF`.
6. Select all TMP_Text components in the scene → change **Font Asset** to `Nunito-Bold SDF`.

---

## 2. Camera / Background Color

1. Select your **Main Camera** in the Hierarchy.
2. Set **Background** color to `#F7F8FA` (hex: F7F8FA, opacity 255).

---

## 3. Dot Grid Background

1. In your **Canvas → InGame UI**, create: **UI → Raw Image**.
2. Rename it `GridDotsBackground`. Stretch it to fill the puzzle area (anchor all sides).
3. Add the `GridDotsPainter` script component.
4. Set **Columns = 10**, **Rows = 18**, **Dot Radius = 5**.
5. Move it **behind** the arrows (`SetAsFirstSibling`).

---

## 4. Arrow Prefab — ArrowMaterialTheme

For **every arrow prefab** in `Assets/_Game/Prefabs/`:

1. Open the prefab.
2. Select the child that has the **LineRenderer**.
3. Add the `ArrowMaterialTheme` script.
4. The LineRenderer material must use a vertex-color-compatible shader:
   - Use **Sprites/Default** or **Unlit/Color**.
5. No further Inspector fields needed — colors come from `ArrowNookTheme`.

Then update your `Line.cs` / `LineClick.cs` to call:
```csharp
GetComponent<ArrowMaterialTheme>()?.SetActive();   // when arrow is tapped
GetComponent<ArrowMaterialTheme>()?.ResetToDefault(); // when released
GetComponent<ArrowMaterialTheme>()?.SetHint();     // from hint system
```

---

## 5. HUD Panel — Move-Count Pill (Optional)

If you want the `🚀 42` move-count pill badge like the reference app:

1. Open your **HUD prefab**.
2. Add a UI **Image** child (pill shape, rounded rect) → assign it to `_moveCountPillImage`.
3. Add a **TMP_Text** child inside it → assign to `_moveCountText`.
4. Call `HUDPanel.Instance.SetMoveCount(moves)` from wherever you count moves.

---

## 6. Win Screen — WinScreenTheme

1. Open your **Win/Results panel prefab**.
2. Add the `WinScreenTheme` script to the **root** of the panel.
3. Assign:
   - **Next Level Button** → your "Next Level" button
   - **Main Menu Button** → your "Main Menu" / "Back" button
   - **Title Label** → the TMP_Text that shows "Level Completed!"
   - **Confetti Root** → leave blank (defaults to the panel root)

---

## 7. Heart Colors

No extra steps needed — `HeartUI` now tints images automatically using `ArrowNookTheme`.
If you use a **single white heart sprite** for both states, the tint handles red vs. gray.

---

## 8. Level Label

Attach `LevelDisplayHelper` to your Level Number TMP_Text in the HUD.
It will auto-show "Level 42" or "Daily Challenge" each frame.

---

## 9. Settings Screen

Already done — `SettingsCredits` adds the About section at runtime. No Inspector changes needed.

---

## Summary of new scripts

| Script | Location |
|---|---|
| `ArrowNookTheme` | Central color palette |
| `ArrowMaterialTheme` | On each arrow LineRenderer child |
| `GridDotsPainter` | On a Raw Image behind the puzzle board |
| `WinScreenTheme` | On the win/results panel root |
| `LevelDisplayHelper` | On the Level Number TMP_Text in HUD |
| `DailyChallenge` | Called from main menu to get today's puzzle |
| `InterstitialAdController` | On the same GameObject as ResultsBannerController |

_Developer: Anupam Pradhan, India — ArrowNook: Logic Puzzle_
