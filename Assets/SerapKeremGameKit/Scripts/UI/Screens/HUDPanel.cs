using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.UI;

namespace SerapKeremGameKit._UI
{
    public sealed class HUDPanel : UIPanel
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private UIRootController _uiRoot;
        [SerializeField] private HeartPanel _heartPanel;

        private void Awake()
        {
            if (_restartButton != null) _restartButton.BindOnClick(this, OnRestartClicked);
            if (_settingsButton != null) _settingsButton.BindOnClick(this, OnSettingsClicked);
            
            if (_heartPanel == null)
            {
                _heartPanel = GetComponentInChildren<HeartPanel>(true);
            }
        }

        private void Start()
        {
            SubscribeToLivesManager();
            InitializeHeartPanel();
        }

        public override void Show(bool playSound = true)
        {
            base.Show(playSound);
            SubscribeToLivesManager();
            InitializeHeartPanel();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnsubscribeFromLivesManager();
            // Auto-unsubscribe handled by ButtonExtensions
        }

        private void SubscribeToLivesManager()
        {
            if (LivesManager.IsInitialized)
            {
                LivesManager.Instance.OnLivesChanged += HandleLivesChanged;
            }
        }

        private void UnsubscribeFromLivesManager()
        {
            if (LivesManager.IsInitialized)
            {
                LivesManager.Instance.OnLivesChanged -= HandleLivesChanged;
            }
        }

        private void InitializeHeartPanel()
        {
            if (_heartPanel != null)
            {
                _heartPanel.Initialize();
                if (LivesManager.IsInitialized)
                {
                    _heartPanel.UpdateHearts(LivesManager.Instance.CurrentLives);
                }
            }
        }

        private void HandleLivesChanged(int currentLives)
        {
            if (_heartPanel != null)
            {
                _heartPanel.UpdateHearts(currentLives);
            }
        }

        public void SetLevelIndex(int levelIndex)
        {
            if (_levelText != null)
                _levelText.text = $"Level {levelIndex + 1}";
        }

        public void UpdateTimeDisplay(float remainingTime)
        {
            if (_timeText != null)
            {
                _timeText.text = FormatTime(remainingTime);
            }
        }

        private static string FormatTime(float seconds)
        {
            int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(seconds));
            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;

            return $"{minutes:D2}:{remainingSeconds:D2}";
        }

        private void OnRestartClicked()
        {
            if (_uiRoot != null) _uiRoot.OnRestartRequested();
        }

        private void OnSettingsClicked()
        {
            if (_uiRoot != null) _uiRoot.OnOpenSettings();
        }

        public void SetUIRoot(UIRootController uiRoot)
        {
            _uiRoot = uiRoot;
        }
    }
}