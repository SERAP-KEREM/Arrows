using UnityEngine;

namespace SerapKeremGameKit._Levels
{
    public sealed class LevelConfig : MonoBehaviour
    {
        [SerializeField] private float[] _timeThresholdsSec = new float[3] { 30f, 45f, 60f };

        [Header("Star Evaluation by Lives")]
        [SerializeField] private int[] _livesThresholds = new int[3] { 5, 3, 1 };

        [Header("Rewards")]
        [SerializeField] private int _winCoins = 10;
        [SerializeField] private int _failCoins = 0;

        public float[] TimeThresholdsSec { get => _timeThresholdsSec; set => _timeThresholdsSec = value; }
        public int[] LivesThresholds { get => _livesThresholds; set => _livesThresholds = value; }
        public int WinCoins { get => _winCoins; set => _winCoins = value; }
        public int FailCoins { get => _failCoins; set => _failCoins = value; }
    }
}
