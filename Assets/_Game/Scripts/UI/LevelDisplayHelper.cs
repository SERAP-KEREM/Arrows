using ArrowNook.Puzzles;
using SerapKeremGameKit._Managers;
using TMPro;
using UnityEngine;

namespace ArrowNook.UI
{
    /// <summary>
    /// Keeps a TextMeshPro label up-to-date with the correct level string.
    ///
    /// • Authored levels 1-10:    "Level 1", "Level 2", …
    /// • Procedural levels 11+:   "Level 11", "Level 12", …  (still readable)
    /// • Daily challenge:         "Daily Challenge"
    ///
    /// Place this component on any TMP_Text in your HUD. It polls once per second
    /// so it catches level transitions without needing event wiring.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LevelDisplayHelper : MonoBehaviour
    {
        [Tooltip("Override label when a daily-challenge puzzle is active.")]
        [SerializeField] private string _dailyLabel = "Daily Challenge";

        [Tooltip("Format for numbered levels. {0} = level number.")]
        [SerializeField] private string _levelFormat = "Level {0}";

        private TMP_Text _label;
        private int _lastShownNumber = -1;
        private bool _lastWasDaily;
        private float _nextCheck;

        private void Awake() => _label = GetComponent<TMP_Text>();

        private void OnEnable() { _lastShownNumber = -1; Refresh(); }

        private void Update()
        {
            if (Time.unscaledTime < _nextCheck) return;
            _nextCheck = Time.unscaledTime + 1f;
            Refresh();
        }

        private void Refresh()
        {
            if (_label == null) return;

            // Check if a daily-challenge level is active (daily levels start at 1001)
            int current = LevelManager.IsInitialized && LevelManager.Instance != null
                ? LevelManager.Instance.ActiveLevelNumber
                : 1;

            bool isDaily = current >= DailyChallenge.DateToLevelNumber(System.DateTime.Now) - 1 &&
                           current <= DailyChallenge.DateToLevelNumber(System.DateTime.Now);

            if (current == _lastShownNumber && isDaily == _lastWasDaily) return;

            _lastShownNumber = current;
            _lastWasDaily = isDaily;

            _label.text = isDaily
                ? _dailyLabel
                : string.Format(_levelFormat, current);
        }

        /// <summary>Force an immediate refresh (call from level-load events).</summary>
        public void ForceRefresh()
        {
            _lastShownNumber = -1;
            Refresh();
        }
    }
}
