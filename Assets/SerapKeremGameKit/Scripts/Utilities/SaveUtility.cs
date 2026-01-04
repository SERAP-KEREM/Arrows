using UnityEngine;

namespace SerapKeremGameKit._Utilities
{
    public static class SaveUtility
    {
        private static float _lastSaveTime;
        private const float MinInterval = 0.25f;

        public static void SaveImmediate()
        {
            PlayerPrefs.Save();
            _lastSaveTime = Time.unscaledTime;
        }

        public static void SaveDebounced()
        {
            if (Time.unscaledTime - _lastSaveTime >= MinInterval)
            {
                PlayerPrefs.Save();
                _lastSaveTime = Time.unscaledTime;
            }
        }
    }
}


