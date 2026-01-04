# SerapKeremGameKit — Unity Game Boilerplate

_A lightweight yet powerful Unity game boilerplate (template) for rapid prototyping and scalable production._

A production‑ready Unity boilerplate/template to kickstart new games fast. It ships with a clean, decoupled architecture, ready‑to‑use managers, pooled audio/particles, UI helpers, haptics, input handling, and editor tooling. Designed for readability, modularity, and scalability. This separation keeps your game logic independent from engine lifecycle, enabling faster iteration and easier testing.

- **Engine**: Unity (URP supported)
- **Language**: C#


## Features

### Core Systems
- 🎵 **Audio System** — Key‑based SFX/music, optional 3D, pooling, PlayerPrefs.
- 💨 **Particle System** — Key‑based pooled particles with auto‑recycle.
- 📱 **Haptics** — Cross‑platform feedback API with enable/disable and cooldown.
- 🖥️ **UI Core** — Animated `UIPanel` base (DOTween fades), audio/haptic hooks.
- 🧠 **Input Handler** — Centralized input with lock/unlock and `PlayerInputSO`.

### Utilities & Architecture
- 💰 **Economy** — Secure `CurrencyWallet` (PlayerPrefs + signature) to deter tampering.
- ⚙️ **Managers** — Lightweight, safe `MonoSingleton<T>` pattern with guards.
- 🪶 **Utilities** — Colored logger, save debounce, preference keys.
- 🧩 **UI Utilities** — `Button.BindOnClick(owner, action)` with auto‑unbind.
- 🧰 **Prefabs** — Ready‑to‑drop manager/UI prefabs under `Resources/*`.

> 🎬 Run the sample scene to see audio, haptics, and UI fades in action.


## Getting Started

### 1) Use as Template (Recommended)
- Click “Use this template” on GitHub to generate your own repository.
- Or fork and rename your project repo.
- Suggested repo topics for discoverability: `unity`, `boilerplate`, `unity-template`, `game-template`, `unity-urp`.

### Alternative: Clone/Import
- Clone the repo into your Unity project `Assets` or import as a template.
- Open with a Unity version compatible with URP.

### 2) Open Sample
- Open `Assets/_Game/Scenes/GameScene.unity` and press Play.

### 3) Prerequisite: Managers Setup
- Keep provided prefabs in scene or instantiate at runtime:
  - `Resources/Managers/AudioManager.prefab`
  - `Resources/Managers/HapticManager.prefab`
  - `Resources/Managers/ParticleManager.prefab`
  - `Resources/Managers/GameManager.prefab`
  - `Resources/Managers/LevelManager.prefab` (if used)
  - `Resources/UI/UI.prefab` (root UI, if used)


## Folder Structure (key parts)
```
Assets/SerapKeremGameKit/
  Resources/
    Managers/            # Drop‑in manager prefabs
    UI/                  # Common UI prefabs (HUD, Settings, etc.)
    Particle/            # Particle pool/player prefabs
    Audio/               # Audio pool/player prefabs
  Scripts/
    Audio/               # AudioManager, AudioData, pooling
    Particles/           # ParticleManager, data, pooling
    Haptics/             # HapticManager, HapticType
    UI/Core/             # UIPanel base (DOTween fades)
    UI/Utilities/        # ButtonExtensions
    InputSystem/         # InputHandler + PlayerInputSO
    Economy/             # CurrencyWallet
    LevelSystem/         # GameManager, State/Level managers
    Utilities/           # TraceLogger, SaveUtility, PreferencesKeys
    Singleton/           # MonoSingleton
  Scenes/SampleScene.unity
```


## Usage Examples

### Audio
Register clips on `AudioManager` via the inspector using `AudioData` entries with `Key` and `Clip`.

```csharp
// Play by key
AudioManager.Instance.Play("ui_click");

// Play 3D at position
AudioManager.Instance.PlayAt("coin_tick", transform.position);

// Music
AudioManager.Instance.PlayMusic(backgroundClip);
AudioManager.Instance.StopMusic();

// User settings
AudioManager.Instance.SetEnabled(true); // persists in PlayerPrefs
bool enabled = AudioManager.Instance.IsEnabled();
```

### Particles
Create `ParticleData` list on `ParticleManager` and call by key.

```csharp
ParticleManager.Instance.PlayParticle("coin_pop", transform.position);
```

### Haptics
Enable/disable and play with basic types.

```csharp
HapticManager.Instance.SetEnabled(true);
HapticManager.Instance.SetGlobalIntensity(0.8f);
HapticManager.Instance.Play(HapticType.Light);
```

### UI Panels (DOTween fades + audio/haptics)
`UIPanel` handles fade in/out and optional audio/haptics hooks.

```csharp
public class SettingsPanel : UIPanel
{
    // Call Show()/Hide() to animate and play hooks
}
```

### Button utility (auto‑unbind)

```csharp
public class MyUIButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.BindOnClick(this, OnClicked); // auto removes listener on destroy
    }

    private void OnClicked() { /* ... */ }
}
```

### Input Handling
Attach `InputHandler` and assign `PlayerInputSO`.

```csharp
if (InputHandler.IsInitialized)
{
    InputHandler.Instance.LockInput();
    // ...
    InputHandler.Instance.UnlockInput();
}
```

### Wallet

```csharp
CurrencyWallet.Instance.Add(100);
// PlayerPrefs + signature (tamper‑safe) storage
bool ok = CurrencyWallet.Instance.TrySpend(50);
int total = CurrencyWallet.Instance.Coins;
```


## Editor & Utilities
- **TraceLogger**: Colored logs with caller info in Editor/Development builds.
- **Toolbars**: Time scale/level toolbars under `Scripts/Editor` (if included).
- **URP Setup**: `GameManager` reads current URP asset and applies shadow distance.

## Using This Boilerplate
- Click “Use this template” on GitHub to create a new repo from this boilerplate.
- Replace company/game identifiers (namespace, product name) in `ProjectSettings` as needed.
- Swap sample assets with your own; keep managers and core systems.
- Add your own gameplay modules in `Scripts/` and hook into the provided managers.


## Dependencies
(All dependencies are optional but recommended for full feature set.)
- TextMeshPro
- TriInspector
- Toolbar Extender
- DOTween
- Toony Colors Pro
- Epic Toon FX
- GUI Blue Sky
- Unity URP (project SRP)

If a package is missing, import it or remove related usages.


## Roadmap
- Addressables integration for audio/particles
- Mobile haptic plugins (advanced patterns)
- Sample gameplay loop and more UI screens


## Contributing
Pull requests welcome. Please follow the project code style: descriptive names, clear structure, and avoid deep nesting. Use consistent naming (PascalCase for classes, camelCase for fields) and avoid allocations in runtime loops. Prefer pooling for frequently spawned objects.


## 📜 **License**  
This project is licensed under the MIT License - see the [LICENSE](https://github.com/SERAP-KEREM/SERAP-KEREM/blob/main/MIT%20License.txt) file for details.
