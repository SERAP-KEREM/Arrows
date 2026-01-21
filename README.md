# 🎯➡️ Arrows

**Arrows** is a challenging line-based puzzle game where players strategically activate animated lines to clear the playfield. Avoid collisions, manage your lives, and complete all levels.
Developed with **Unity**, the game features smooth animations, intelligent collision detection, and a robust, scalable architecture.

---

## 🎮 About the Game

Arrows is a line-based puzzle game where players activate lines by clicking on them.
Each line moves forward with an animation while gradually disappearing from its tail.

The core challenge lies in **timing your clicks correctly** to prevent collisions between lines.

* When two lines collide, you lose a life.
* Clear all lines to win the level.

---

## 🎥 Gameplay Video

[https://github.com/user-attachments/assets/d94b12ce-ee31-4cbf-88be-0a3252be2a14](https://github.com/user-attachments/assets/d94b12ce-ee31-4cbf-88be-0a3252be2a14)

---

## 🖼️ Screenshots

<p align="center">

  <img src="https://github.com/SERAP-KEREM/Arrows/blob/main/Assets/GameImages/1.png?raw=true" alt="Game Screenshot 1" width="300">

  <img src="https://github.com/SERAP-KEREM/Arrows/blob/main/Assets/GameImages/2.png?raw=true" alt="Game Screenshot 2" width="300">

</p>

<p align="center">

  <img src="https://github.com/SERAP-KEREM/Arrows/blob/main/Assets/GameImages/3.png?raw=true" alt="Game Screenshot 3" width="300">

  <img src="https://github.com/SERAP-KEREM/Arrows/blob/main/Assets/GameImages/4.png?raw=true" alt="Game Screenshot 4" width="300">

</p>

---

## ✨ Game Features

### 🎯 Core Mechanics

* ➡️ **Interactive Line System**
  Activate precise, smooth forward animations by clicking on lines.
* 💥 **Smart Collision Detection**
  Advanced head-to-line collision system.
* ❤️ **Lives Management**
  Start with 5 lives and track remaining lives via heart-based UI.
* 🏆 **Win / Lose Conditions**
  Clear all lines to win; lose all lives to fail the level.
* 📊 **Level Progression**
  10 carefully designed levels with increasing difficulty.

---

### 🎨 Visual Features

* 🎬 **Smooth Animations**
  DOTween-powered forward and backward line animations.
* 🎨 **Dynamic Color Feedback**
  Lines change color on collision to clearly indicate errors.
* 📹 **Automatic Camera Adjustment**
  Camera automatically frames all lines per level.
* ✨ **Line Head Tracking**
  A visual “head” object follows the line tip for better visibility.
* 🎞️ **Material System**
  Dynamic material and color management for visual feedback.

---

## 🧠 Technical Features

* 🧱 **Component-Based Architecture**
  Modular, SOLID-compliant design with clear separation of responsibilities.
* ⚡ **Vector3 Array Pooling**
  Zero-allocation animation system optimized for performance.
* 🔄 **State Management**
  Centralized game state control via a `StateManager`.
* 🔊 **Audio & Haptics**
  Sound effects and tactile feedback support.
* 📂 **Level System**
  Flexible prefab-based level loading and flow control.
* 🎛️ **Explicit Initialization**
  Clear, deterministic initialization order instead of Unity’s implicit lifecycle.


---

## 🛠️ Tools & Packages Used

### 📦 Unity Packages

- ⚙️ **Unity Engine** — 6000.0.58f2 (Unity 6)
- 🔄 **DOTween** — Tween-based animations for line movement  
- 🧰 **TriInspector** — Advanced Inspector UI for efficient development  
- 🎨 **Universal Render Pipeline (URP)** — Modern and optimized rendering  
- 📝 **TextMeshPro** — Advanced text rendering for UI  
- ➰ **Line Renderer** — Core system for rendering and animating dynamic lines

---

### 🧩 Custom Framework

**SerapKeremGameKit** – Production-ready Unity infrastructure:

* 📝 Logging and tracing system
* 🔊 Pooling-based audio management
* 📳 Cross-platform haptic support
* ✨ Auto-recycling particle system
* ♻️ State-driven level system
* 🖼️ Panel-based UI framework
* 🔄 Game state management system
* 💰 Currency / wallet system
* 🧰 Guarded MonoSingleton architecture

---

## 🎨 Custom Systems

### ➡️ Line System

A fully custom-built line architecture including:

* 🎬 **LineAnimation**
  Forward/backward animation using array pooling (zero allocation)
* 👆 **LineClick**
  Input handling and line activation logic
* 💥 **LineHeadCollisionDetector**
  Precise collision detection between line heads and bodies
* 🎨 **LineMaterialHandler**
  Dynamic color management for visual feedback
* 🗑️ **LineDestroyer**
  Automatic cleanup after animation completion
* ➡️ **LineRendererHead**
  Visual head object that follows the line’s endpoint

---

### 🎛️ Game Systems

* ❤️ **LivesManager** — Singleton-based life management
* 📹 **CameraManager** — Automatic camera adjustment based on level bounds
* 🎯 **Level System** — Prefab-based loading with explicit initialization
* 🔄 **StateManager** — Centralized game states (`Loading`, `OnStart`, `OnWin`, `OnLose`)

---

## 🎯 How to Play

### 📘 Basic Rules

* 🎯 **Click to Activate**
  Click on any line to activate it.
* 💥 **Avoid Collisions**
  Each collision costs one life.
* ➡️ **Line Completion**
  Lines erase from the tail as they move and are removed after completion.
* ❤️ **Manage Your Lives**
  You start with 5 lives.
* 🏆 **Win Condition**
  Complete all lines without collisions.
* 💔 **Lose Condition**
  Lose all 5 lives.

---

### 🕹️ Controls

* 🖱️ **Mouse / Touch** — Click or tap a line to activate
* ⏸️ **No Re-activation** — Moving lines cannot be activated again
* 🎯 **Strategy** — Analyze line placement carefully before clicking

---

## 📦 Project Structure

```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Line/
│   │   └── UI/
│   ├── Resources/
│   │   ├── Levels/
│   │   └── Line/
│   ├── Scenes/
│   │   └── GameScene.unity
│   └── ...
└── SerapKeremGameKit/
```

---

## 🚀 Getting Started

### 📥 Installation

```bash
git clone https://github.com/SERAP-KEREM/Arrows.git
```

1. Open the project in **Unity Hub**
2. Open the main scene:
   `Assets/_Game/Scenes/GameScene.unity`
3. Press **Play**

---

### 🛠️ Build

1. Go to **File → Build Settings**
2. Select the target platform
3. Click **Build**

---

## 📜 **License**

This project is licensed under the MIT License - see the [LICENSE](https://github.com/SERAP-KEREM/SERAP-KEREM/blob/main/MIT%20License.txt) file for details.

