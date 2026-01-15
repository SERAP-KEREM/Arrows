using SerapKeremGameKit._Economy;
using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Levels;
using SerapKeremGameKit._Managers;
using SerapKeremGameKit._Time;
using UnityEngine;
using SerapKeremGameKit._Audio;
using SerapKeremGameKit._Haptics;
using _Game.UI;

namespace SerapKeremGameKit._UI
{
    public sealed class UIRootController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HUDPanel _hud;
        [SerializeField] private WinPanel _win;
        [SerializeField] private FailPanel _fail;
        [SerializeField] private SettingsPanel _settings;
        [SerializeField] private RetryPanel _retry;

        [Header("Data")]
        [SerializeField] private LevelConfig _fallbackConfig;

        private GameState _lastState = GameState.None;

        [Header("Audio Keys")]
        [SerializeField] private string _keyOnWin = "game_win";
        [SerializeField] private string _keyOnLose = "game_lose";
        [SerializeField] private string _keyOnOpenSettings = "ui_open";
        [SerializeField] private string _keyOnRestartDialog = "ui_open";
        [SerializeField] private string _keyOnRestartConfirm = "ui_confirm";
        [SerializeField] private string _keyOnNext = "ui_next";

        private void Awake()
        {
            // Auto-wire if not assigned
            if (_hud == null) _hud = GetComponentInChildren<HUDPanel>(true);
            if (_win == null) _win = GetComponentInChildren<WinPanel>(true);
            if (_fail == null) _fail = GetComponentInChildren<FailPanel>(true);
            if (_settings == null) _settings = GetComponentInChildren<SettingsPanel>(true);
            if (_retry == null) _retry = GetComponentInChildren<RetryPanel>(true);

            // Inject UIRoot into screens to avoid FindObjectOfType
            if (_hud != null) _hud.SetUIRoot(this);
            if (_win != null) _win.SetUIRoot(this);
            if (_fail != null) _fail.SetUIRoot(this);
            if (_retry != null) _retry.SetUIRoot(this);

            // Ensure startup state: only HUD hidden initially (will be shown in Start)
            if (_win != null) _win.HideImmediate();
            if (_fail != null) _fail.HideImmediate();
            if (_settings != null) _settings.HideImmediate();
            if (_retry != null) _retry.HideImmediate();
            if (_hud != null) _hud.HideImmediate();
        }

        private void Start()
        {
            ApplyInitialState();
        }

        private void Update()
        {
            SyncWithGameState();
        }

        private void ApplyInitialState()
        {
            HideAll();
            InitializeHUD();
        }

        public void InitializeHUD()
        {
            if (_hud != null)
            {
                _hud.Show(false);
                _hud.SetLevelIndex(LevelManager.Instance.ActiveLevelNumber - 1);
            }
        }

        private void SyncWithGameState()
        {
            GameState current = StateManager.Instance.CurrentState;
            if (current == _lastState) return;
            _lastState = current;

            if (current == GameState.OnStart)
            {
                HideAll();
                if (_hud != null)
                {
                    _hud.Show(false);
                    _hud.SetLevelIndex(LevelManager.Instance.ActiveLevelNumber - 1);
                }
            }
            else if (current == GameState.OnWin)
            {
                ShowWin();
                if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnWin)) AudioManager.Instance.Play(_keyOnWin);
                if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Success);
            }
            else if (current == GameState.OnLose)
            {
                ShowFail();
                if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnLose)) AudioManager.Instance.Play(_keyOnLose);
                if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Failure);
            }
        }

        private void HideAll()
        {
            if (_hud != null) _hud.Hide(false);
            if (_win != null) _win.Hide(false);
            if (_fail != null) _fail.Hide(false);
            if (_settings != null) _settings.Hide(false);
            if (_retry != null) _retry.Hide(false);
        }

        private void ShowWin()
        {
            HideExcept(_win);
            if (_win != null)
            {
                Level active = LevelManager.Instance.ActiveLevelInstance;
                LevelConfig config = ResolveConfig(active);

                int stars = StarEvaluator.EvaluateStarsByLives();
                int reward = Mathf.Max(0, config != null ? config.WinCoins : 10);
                int totalBefore = 0;
                if (CurrencyWallet.Instance != null)
                {
                    totalBefore = CurrencyWallet.Instance.Coins;
                }
                _win.Setup(stars, reward, totalBefore, this);
                _win.Show();
            }
        }

        
        private float CalculateCompletionTime(Level level)
        {
            if (level == null)
            {
                return 0f;
            }

            float levelTime = level.LevelTime;
            float remainingTime = 0f;

            if (TimeManager.IsInitialized)
            {
                remainingTime = TimeManager.Instance.RemainingTime;
            }

            float completionTime = Mathf.Max(0f, levelTime - remainingTime);
            return completionTime;
        }

        private void ShowFail()
        {
            HideExcept(_fail);
            if (_fail != null)
            {
                Level active = LevelManager.Instance.ActiveLevelInstance;
                LevelConfig config = ResolveConfig(active);
                int reward = Mathf.Max(0, config != null ? config.FailCoins : 0);
                if (reward > 0 && CurrencyWallet.Instance != null) CurrencyWallet.Instance.Add(reward);
                _fail.Setup(reward, this);
                _fail.Show();
            }
        }

        private LevelConfig ResolveConfig(Level level)
        {
            if (level != null)
            {
                // Try to get LevelConfig from Level GameObject or its children
                LevelConfig attached = level.GetComponent<LevelConfig>();
                if (attached == null)
                {
                    attached = level.GetComponentInChildren<LevelConfig>(true);
                }
                if (attached != null) return attached;
            }
            return _fallbackConfig;
        }

        private void HideExcept(UIPanel screen)
        {
            if (_hud != null && _hud != screen) _hud.Hide(true);
            if (_win != null && _win != screen) _win.Hide(true);
            if (_fail != null && _fail != screen) _fail.Hide(true);
            if (_settings != null && _settings != screen) _settings.Hide(true);
            if (_retry != null && _retry != screen) _retry.Hide(true);
        }

        public void OnRestartRequested()
        {
            if (_retry != null) _retry.Show();
            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnRestartDialog)) AudioManager.Instance.Play(_keyOnRestartDialog);
            if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Medium);
        }

        public void OnRestartConfirmed()
        {
            HideAll();
            LevelManager.Instance.RestartLevel();
            if (_hud != null)
            {
                _hud.Show(false);
                _hud.SetLevelIndex(LevelManager.Instance.ActiveLevelNumber - 1);
            }
            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnRestartConfirm)) AudioManager.Instance.Play(_keyOnRestartConfirm);
            if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Medium);
        }

        public void OnNextLevelRequested()
        {
            HideAll();
            if (_hud != null)
            {
                _hud.Show(false);
            }
            LevelManager.Instance.IncreaseLevelNumber();
            LevelManager.Instance.LoadCurrentLevel();
            if (_hud != null)
            {
                _hud.SetLevelIndex(LevelManager.Instance.ActiveLevelNumber - 1);
            }
            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnNext)) AudioManager.Instance.Play(_keyOnNext);
            if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Light);
        }

        public void ProceedNextLevelAfterReward(int reward)
        {
            if (CurrencyWallet.Instance != null && reward > 0)
            {
                CurrencyWallet.Instance.Add(reward);
            }
            OnNextLevelRequested();
        }

        public void OnOpenSettings()
        {
            if (_settings != null) _settings.Show();
            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnOpenSettings)) AudioManager.Instance.Play(_keyOnOpenSettings);
            if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Selection);
        }


        public void UpdateTimeDisplay(float remainingTime)
        {
            if (_hud != null)
            {
                _hud.UpdateTimeDisplay(remainingTime);
            }
        }

        public LivesManager LivesManagerInstance
        {
            get
            {
                if (LivesManager.IsInitialized)
                {
                    return LivesManager.Instance;
                }
                return null;
            }
        }
    }
}