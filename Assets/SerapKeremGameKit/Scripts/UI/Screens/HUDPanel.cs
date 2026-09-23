using ArrowNook.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.UI;
using SerapKeremGameKit._InputSystem;
using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Managers;

namespace SerapKeremGameKit._UI
{
    public sealed class HUDPanel : UIPanel
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _hintButton;
        [SerializeField] private TextMeshProUGUI _hintStatus;
        [SerializeField] private UIRootController _uiRoot;
        [SerializeField] private HeartPanel _heartPanel;

        [Header("Move-count pill (optional)" )]
        [Tooltip("The Image behind the move-count label. Gets the theme pill background color.")]
        [SerializeField] private Image _moveCountPillImage;
        [Tooltip("The TMP label showing the move count. Gets the theme pill foreground color.")]
        [SerializeField] private TextMeshProUGUI _moveCountText;

        private bool _isInitialized = false;
        private float _hintStatusUntil;
        private int _moveCount;
        private RectTransform _header;
        private readonly Vector3[] _corners = new Vector3[4];

        private void Awake()
        {
            _header = transform.Find("Header") as RectTransform;
            ApplySafeArea();
            if (_restartButton != null) _restartButton.BindOnClick(this, OnRestartClicked);
            if (_settingsButton != null) _settingsButton.BindOnClick(this, OnSettingsClicked);
            if (_hintButton != null) _hintButton.BindOnClick(this, OnHintClicked);
            ApplyThemeColors();
        }

        /// <summary>Apply ArrowNook theme colors to all HUD elements.</summary>
        private void ApplyThemeColors()
        {
            // Level label
            if (_levelText != null)
                _levelText.color = ArrowNookTheme.LabelPrimary;

            // Hint status text
            if (_hintStatus != null)
                _hintStatus.color = ArrowNookTheme.LabelPrimary;

            // Time text
            if (_timeText != null)
                _timeText.color = ArrowNookTheme.LabelSecondary;

            // Move-count pill
            if (_moveCountPillImage != null)
                _moveCountPillImage.color = ArrowNookTheme.PillBackground;
            if (_moveCountText != null)
                _moveCountText.color = ArrowNookTheme.PillForeground;
        }

        /// <summary>Update the move-count pill with the current number of moves used.</summary>
        public void SetMoveCount(int moves)
        {
            _moveCount = moves;
            if (_moveCountText != null)
                _moveCountText.text = moves.ToString();
        }

        private void Update()
        {
            if (_hintButton == null) return;
            Level level = LevelManager.IsInitialized ? LevelManager.Instance.ActiveLevelInstance : null;
            _hintButton.interactable = StateManager.IsInitialized &&
                StateManager.Instance.CurrentState == GameState.OnStart &&
                (!InputHandler.IsInitialized || !InputHandler.Instance.IsInputLocked) &&
                level != null && level.LineManager != null && !level.LineManager.HasMovingLines;
            if (_hintStatus != null && Time.unscaledTime >= _hintStatusUntil)
                _hintStatus.text = "Hint";
        }

        private void OnHintClicked()
        {
            if (_hintButton == null || !_hintButton.interactable || !LevelManager.IsInitialized) return;
            Level level = LevelManager.Instance.ActiveLevelInstance;
            if (level == null || level.LineManager == null) return;
            if (level.LineManager.TryGetHint(out var hint)) hint.ShowHint();
            else if (_hintStatus != null)
            {
                _hintStatus.text = "No clear exit";
                _hintStatusUntil = Time.unscaledTime + 2f;
            }
        }

        public override void Show(bool playSound = true)
        {
            base.Show(playSound);
            _hintStatusUntil = 0f;
            if (_hintStatus != null) _hintStatus.text = "Hint";
            
            if (!_isInitialized)
            {
                Initialize();
            }
            
            SubscribeToLivesManager();
            InitializeHeartPanel();
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnsubscribeFromLivesManager();
            // Auto-unsubscribe handled by ButtonExtensions
        }

        private void SubscribeToLivesManager()
        {
            if (_uiRoot != null && _uiRoot.LivesManagerInstance != null)
            {
                _uiRoot.LivesManagerInstance.OnLivesChanged -= HandleLivesChanged;
                _uiRoot.LivesManagerInstance.OnLivesChanged += HandleLivesChanged;
            }
        }

        private void UnsubscribeFromLivesManager()
        {
            if (_uiRoot != null && _uiRoot.LivesManagerInstance != null)
            {
                _uiRoot.LivesManagerInstance.OnLivesChanged -= HandleLivesChanged;
            }
        }

        private void InitializeHeartPanel()
        {
            if (_heartPanel != null)
            {
                _heartPanel.Initialize();
                if (_uiRoot != null && _uiRoot.LivesManagerInstance != null)
                {
                    _heartPanel.UpdateHearts(_uiRoot.LivesManagerInstance.CurrentLives);
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
            SetLevelLabel($"Level {levelIndex + 1}");
        }

        /// <summary>
        /// Sets the level label to any string (e.g. "Level 42" or "Daily Challenge").
        /// </summary>
        public void SetLevelLabel(string label)
        {
            if (_levelText != null)
            {
                _levelText.enableAutoSizing = true;
                _levelText.fontSizeMin = 20;
                _levelText.color = ArrowNookTheme.LabelPrimary;
                _levelText.text = label;
            }
        }

        public Rect GetBoardScreenRect()
        {
            ApplySafeArea();
            Canvas.ForceUpdateCanvases();
            Rect area = Screen.safeArea;
            Canvas canvas = GetComponentInParent<Canvas>();
            Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            if (_header != null)
            {
                _header.GetWorldCorners(_corners);
                area.yMax = Mathf.Min(area.yMax, RectTransformUtility.WorldToScreenPoint(camera, _corners[0]).y);
            }
            if (_hintButton != null)
            {
                ((RectTransform)_hintButton.transform).GetWorldCorners(_corners);
                area.yMin = Mathf.Max(area.yMin, RectTransformUtility.WorldToScreenPoint(camera, _corners[1]).y);
            }
            return area;
        }

        private void ApplySafeArea()
        {
            if (Screen.width <= 0 || Screen.height <= 0) return;
            var rect = (RectTransform)transform;
            Rect safe = Screen.safeArea;
            rect.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            rect.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
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
