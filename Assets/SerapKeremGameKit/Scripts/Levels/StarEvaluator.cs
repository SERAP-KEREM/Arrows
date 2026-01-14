using _Game.UI;

namespace SerapKeremGameKit._Levels
{
    public static class StarEvaluator
    {
        public static int EvaluateStars(float[] thresholdsSec, float completionTimeSec)
        {
            if (thresholdsSec == null || thresholdsSec.Length < 3)
                return 0;

            float t = completionTimeSec;
            if (t <= thresholdsSec[0]) return 3;
            if (t <= thresholdsSec[1]) return 2;
            if (t <= thresholdsSec[2]) return 1;
            return 0;
        }

        public static int EvaluateStars(LevelConfig config, float completionTimeSec)
        {
            if (config == null || config.TimeThresholdsSec == null || config.TimeThresholdsSec.Length < 3)
                return 0;

            float t = completionTimeSec;
            if (t <= config.TimeThresholdsSec[0]) return 3;
            if (t <= config.TimeThresholdsSec[1]) return 2;
            if (t <= config.TimeThresholdsSec[2]) return 1;
            return 0;
        }

        public static int EvaluateStarsByLives()
        {
            if (!LivesManager.IsInitialized)
                return 0;

            int currentLives = LivesManager.Instance.CurrentLives;

            if (currentLives >= 5) return 3;
            if (currentLives >= 3) return 2;
            if (currentLives >= 1) return 0;
            return 0;
        }
    }
}



