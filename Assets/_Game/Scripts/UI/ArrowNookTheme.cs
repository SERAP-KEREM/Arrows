using UnityEngine;

namespace ArrowNook.UI
{
    /// <summary>
    /// Central color palette for ArrowNook: Logic Puzzle.
    /// Inspired by the clean minimalist look of top arrow puzzle games:
    /// • Off-white background with subtle gray dot grid
    /// • Deep navy arrows (high contrast, readable on small screens)
    /// • Electric blue for active/selected states
    /// • Red hearts, blue pill badges — familiar and approachable
    ///
    /// All C# scripts that need colors read from this class.
    /// Unity Inspector overrides (materials, Image colors) are listed in
    /// docs/ui-inspector-setup.md.
    /// </summary>
    public static class ArrowNookTheme
    {
        // ── Background ─────────────────────────────────────────────────────────
        /// <summary>Main puzzle-board background: cool off-white (#F7F8FA).</summary>
        public static readonly Color Background = Hex("#F7F8FA");

        /// <summary>Subtle grid-dot color overlaid on the board (#D1D9E0).</summary>
        public static readonly Color GridDot = Hex("#D1D9E0");

        // ── Arrows ─────────────────────────────────────────────────────────────
        /// <summary>Default arrow line color: deep navy (#1A2332).</summary>
        public static readonly Color ArrowDefault = Hex("#1A2332");

        /// <summary>Arrow selected / currently sliding: electric blue (#007AFF).</summary>
        public static readonly Color ArrowActive = Hex("#007AFF");

        /// <summary>Hint-highlighted arrow: sky blue (#00A3FF).</summary>
        public static readonly Color ArrowHint = Hex("#00A3FF");

        // ── HUD ────────────────────────────────────────────────────────────────
        /// <summary>Level label and heading text: dark navy (#1A2332).</summary>
        public static readonly Color LabelPrimary = Hex("#1A2332");

        /// <summary>Secondary / caption text: medium slate (#64748B).</summary>
        public static readonly Color LabelSecondary = Hex("#64748B");

        /// <summary>Pill badge surface (move count, difficulty): light gray (#E8EDF2).</summary>
        public static readonly Color PillBackground = Hex("#E8EDF2");

        /// <summary>Pill badge icon / text color: dark slate (#334155).</summary>
        public static readonly Color PillForeground = Hex("#334155");

        // ── Interactives ───────────────────────────────────────────────────────
        /// <summary>Primary button surface: electric blue (#007AFF).</summary>
        public static readonly Color ButtonPrimary = Hex("#007AFF");

        /// <summary>Primary button text: white.</summary>
        public static readonly Color ButtonPrimaryText = Color.white;

        /// <summary>Floating action button surface: white with shadow.</summary>
        public static readonly Color FloatingButton = Color.white;

        /// <summary>Hint badge bubble: electric blue (#007AFF).</summary>
        public static readonly Color HintBadge = Hex("#007AFF");

        // ── Lives ──────────────────────────────────────────────────────────────
        /// <summary>Active heart / life: crimson red (#FF3B30).</summary>
        public static readonly Color HeartActive = Hex("#FF3B30");

        /// <summary>Depleted heart: light gray (#C8CDD4).</summary>
        public static readonly Color HeartEmpty = Hex("#C8CDD4");

        // ── Win screen ─────────────────────────────────────────────────────────
        /// <summary>Win-screen header background: vibrant sky blue (#0088FF).</summary>
        public static readonly Color WinBackground = Hex("#0088FF");

        /// <summary>Win-screen sunburst ray color: slightly lighter blue (#33A8FF).</summary>
        public static readonly Color WinRay = Hex("#33A8FF");

        /// <summary>Win-screen "Next Level" button surface: white.</summary>
        public static readonly Color WinButton = Color.white;

        /// <summary>Win-screen "Next Level" button text: electric blue (#007AFF).</summary>
        public static readonly Color WinButtonText = Hex("#007AFF");

        // ── Confetti palette ───────────────────────────────────────────────────
        public static readonly Color[] ConfettiColors =
        {
            Hex("#00C8FF"), // cyan
            Hex("#FF3B8A"), // magenta
            Hex("#FFD600"), // yellow
            Hex("#4CD964"), // lime green
            Hex("#FF9500"), // orange
            Hex("#FFFFFF"), // white
        };

        // ── Helper ─────────────────────────────────────────────────────────────
        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color c);
            return c;
        }

        /// <summary>
        /// Returns a Color with the same RGB as <paramref name="c"/> but a different alpha.
        /// </summary>
        public static Color WithAlpha(Color c, float a) => new Color(c.r, c.g, c.b, a);
    }
}
