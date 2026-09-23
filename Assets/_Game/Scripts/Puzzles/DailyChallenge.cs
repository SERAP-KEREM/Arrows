using System;

namespace ArrowNook.Puzzles
{
    /// <summary>
    /// Provides a deterministic daily-challenge puzzle seeded by calendar date.
    /// The same date always produces the same puzzle regardless of app version,
    /// making it safe to advertise as a repeatable "daily" across reinstalls.
    ///
    /// Usage:
    ///   PuzzleDefinition daily = DailyChallenge.GetTodaysPuzzle();
    ///   // Pass to ProceduralLevelBuilder.Populate just like any PuzzleDefinition.
    /// </summary>
    public static class DailyChallenge
    {
        /// <summary>
        /// Returns the daily puzzle for the current local calendar date.
        /// </summary>
        public static PuzzleDefinition GetTodaysPuzzle() => GetPuzzleForDate(DateTime.Now);

        /// <summary>
        /// Returns the daily puzzle for an arbitrary date (useful for previewing tomorrow's).
        /// </summary>
        public static PuzzleDefinition GetPuzzleForDate(DateTime date)
        {
            int levelNumber = DateToLevelNumber(date);
            return PuzzleGenerator.Generate(levelNumber);
        }

        /// <summary>
        /// Returns the virtual level number for a given date.
        /// Level 1 == 2026-01-01. The number advances by one each day,
        /// wrapping through difficulty tiers just like the endless mode.
        /// </summary>
        public static int DateToLevelNumber(DateTime date)
        {
            // Epoch: 2026-01-01 == daily level 1001 (puts daily challenges in a
            // different difficulty range than the infinite endless levels).
            var epoch = new DateTime(2026, 1, 1);
            int dayOffset = (int)(date.Date - epoch.Date).TotalDays;
            return Math.Max(1, 1001 + dayOffset);
        }

        /// <summary>
        /// Key used to persist whether the player completed today's challenge.
        /// </summary>
        public static string TodayPrefsKey => "daily_done_" + DateTime.Now.ToString("yyyyMMdd");

        /// <summary>Whether the player already finished today's challenge.</summary>
        public static bool CompletedToday
        {
            get => UnityEngine.PlayerPrefs.GetInt(TodayPrefsKey, 0) == 1;
            set
            {
                UnityEngine.PlayerPrefs.SetInt(TodayPrefsKey, value ? 1 : 0);
                UnityEngine.PlayerPrefs.Save();
            }
        }
    }
}
