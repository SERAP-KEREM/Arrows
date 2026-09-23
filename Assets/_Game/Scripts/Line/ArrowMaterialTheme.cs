using ArrowNook.UI;
using UnityEngine;

namespace ArrowNook.Line
{
    /// <summary>
    /// Applies <see cref="ArrowNookTheme"/> colors to a <see cref="LineRenderer"/> at runtime.
    ///
    /// Attach this to every arrow prefab (the same GameObject that has the LineRenderer).
    /// The LineRenderer must use a material that supports vertex colors — the standard
    /// "Sprites/Default" or "Unlit/Color" shader both work. Using "Sprites/Default" also
    /// lets you keep the rounded-cap look with a texture.
    ///
    /// State machine:
    ///   Default  → navy  (#1A2332)
    ///   Active   → electric blue (#007AFF)   – call <see cref="SetActive"/>
    ///   Hint     → sky blue (#00A3FF)        – call <see cref="SetHint"/>
    ///   Reset    → back to Default           – call <see cref="ResetToDefault"/>
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public sealed class ArrowMaterialTheme : MonoBehaviour
    {
        private LineRenderer _line;
        private bool _isHinted;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            Apply(ArrowNookTheme.ArrowDefault);
        }

        // ── Public API ─────────────────────────────────────────────────────────

        /// <summary>Switch to the electric-blue active color.</summary>
        public void SetActive()
        {
            _isHinted = false;
            Apply(ArrowNookTheme.ArrowActive);
        }

        /// <summary>Switch to the sky-blue hint color.</summary>
        public void SetHint()
        {
            _isHinted = true;
            Apply(ArrowNookTheme.ArrowHint);
        }

        /// <summary>Return to default navy color.</summary>
        public void ResetToDefault()
        {
            _isHinted = false;
            Apply(ArrowNookTheme.ArrowDefault);
        }

        /// <summary>True while hint highlight is active.</summary>
        public bool IsHinted => _isHinted;

        // ── Internal ───────────────────────────────────────────────────────────

        private void Apply(Color color)
        {
            if (_line == null) return;
            _line.startColor = color;
            _line.endColor = color;
            // Also tint the material so mesh-renderer-based head sprites match.
            if (_line.material != null)
                _line.material.color = color;
        }
    }
}
