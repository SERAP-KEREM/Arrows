using UnityEngine;

namespace SerapKeremGameKit._Levels
{
    public sealed class LevelConfig : MonoBehaviour
    {
        [SerializeField, Tooltip("Time thresholds (seconds) for 3/2/1 stars. Lower time yields more stars.")]
        private float[] _timeThresholdsSec = new float[3] { 30f, 45f, 60f };

        // [SerializeField] private int[] _moveThresholds = new int[3] { 10, 15, 20 }; // Reserved for future use

        [Header("Rewards")]
        [SerializeField] private int _winCoins = 10;
        [SerializeField] private int _failCoins = 0;

        public float[] TimeThresholdsSec { get => _timeThresholdsSec; set => _timeThresholdsSec = value; }
        // public int[] MoveThresholds { get => _moveThresholds; set => _moveThresholds = value; } // future use
        public int WinCoins { get => _winCoins; set => _winCoins = value; }
        public int FailCoins { get => _failCoins; set => _failCoins = value; }
    }
}
