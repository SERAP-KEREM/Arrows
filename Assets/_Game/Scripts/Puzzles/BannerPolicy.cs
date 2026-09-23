namespace ArrowNook.Puzzles
{
    public static class BannerPolicy
    {
        // ── Banner ─────────────────────────────────────────────────────────────
        public const int FirstAdLevel = 4;          // Do not show ads before this level
        public const double WarmupSeconds = 90;     // Minimum session time before any ad
        public const int WidthPoints = 320;         // Minimum safe-area width for a banner
        public const int HeightPoints = 50;         // Banner height in dp
        public const int SeparationPoints = 32;     // Gap between banner and puzzle content

        public static bool IsEligible(int level, double sessionSeconds, double safeWidthPoints) =>
            level >= FirstAdLevel && sessionSeconds >= WarmupSeconds && safeWidthPoints >= WidthPoints;

        public static double RetryDelaySeconds(int failures) => failures <= 1 ? 60 : failures == 2 ? 120 : 300;

        // ── Interstitial ───────────────────────────────────────────────────────
        /// <summary>Show at most one interstitial per this many levels completed.</summary>
        public const int InterstitialEveryNLevels = 3;

        /// <summary>Minimum seconds between two successive interstitials (even if N levels pass).</summary>
        public const double InterstitialCooldownSeconds = 180;
    }
}
