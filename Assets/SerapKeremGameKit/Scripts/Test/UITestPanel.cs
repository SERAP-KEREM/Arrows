using SerapKeremGameKit._Economy;
using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Levels;
using TriInspector;
using UnityEngine;
using SerapKeremGameKit._Managers;

namespace SerapKeremGameKit._UI
{
    [HideMonoScript]
    public sealed class UITestPanel : MonoBehaviour
    {
        [Title("References")]
        [Group("Refs"), SerializeField] private UIRootController _uiRoot;
		[Group("Refs"), SerializeField] private HUDPanel _hud;
		[Group("Refs"), SerializeField] private WinPanel _win;
		[Group("Refs"), SerializeField] private FailPanel _fail;
		[Group("Refs"), SerializeField] private SettingsPanel _settings;
		[Group("Refs"), SerializeField] private RetryPanel _retry;
        [Group("Refs"), SerializeField] private CurrencyWallet _wallet; // optional reference for tests

        [Title("Star & Reward Test")]
        [Group("StarTest"), SerializeField] private bool _useActiveLevelConfig = true;
        [Group("StarTest"), SerializeField, Tooltip("Manual time thresholds (3/2/1 stars) in seconds.")]
        private float[] _manualTimeThresholdsSec = new float[3] { 30f, 45f, 60f };
        [Group("StarTest"), SerializeField, Min(0f)] private float _completionTimeSec = 35f;
        [Group("StarTest"), SerializeField, Min(0)] private int _manualWinCoins = 10;
        [Group("StarTest"), SerializeField, Min(0)] private int _manualFailCoins = 0;
        [Group("StarTest"), SerializeField, Tooltip("If enabled, adds preview coins to wallet when showing panels. Wallet reference is optional.")]
        private bool _applyRewardsToWallet = false;
        [Group("StarTest"), SerializeField, Tooltip("If enabled, overrides computed stars with the desired value.")]
        private bool _forceStars = false;
        [Group("StarTest"), SerializeField, Range(0, 3)] private int _forcedStars = 3;

        [ShowInInspector, ReadOnly]
        public int PreviewStars { get; private set; }

        [ShowInInspector, ReadOnly]
        public int PreviewWinCoins { get; private set; }

        [ShowInInspector, ReadOnly]
        public int PreviewFailCoins { get; private set; }

        [Button("Compute Preview")]
        private void ComputePreview()
        {
            var (stars, winCoins, failCoins) = ComputeValues();
            PreviewStars = stars;
            PreviewWinCoins = winCoins;
            PreviewFailCoins = failCoins;
        }

        [Title("HUD")]
        [Group("HUDBtns"), Button("Show HUD")]
        private void Btn_ShowHUD()
        {
            if (_hud == null) return;
            _hud.SetLevelIndex(LevelManager.Instance != null ? LevelManager.Instance.ActiveLevelNumber - 1 : 0);
            _hud.Show();
        }

        [Group("HUDBtns"), Button("Hide HUD")]
        private void Btn_HideHUD()
        {
            _hud?.Hide();
        }

        [Title("Win Screen")]
        [Group("WinBtns"), Button("Show Win (From Preview)")]
        private void Btn_ShowWin()
        {
            if (_win == null) return;
            var (stars, winCoins, _) = ComputeValues();
            int total = 0;
            if (_applyRewardsToWallet)
            {
                var w = GetWallet();
                if (w != null)
                {
                    w.Add(winCoins);
                    total = w.Coins;
                }
            }
            _win.Setup(stars, winCoins, total, _uiRoot);
            _win.Show();
        }

        [Group("WinBtns"), Button("Hide Win")]
        private void Btn_HideWin()
        {
            _win?.Hide();
        }

        [Title("Fail Screen")]
        [Group("FailBtns"), Button("Show Fail (From Preview)")]
        private void Btn_ShowFail()
        {
            if (_fail == null) return;
            var (_, _, failCoins) = ComputeValues();
            if (_applyRewardsToWallet && failCoins > 0)
            {
                var w = GetWallet();
                if (w != null) w.Add(failCoins);
            }
            _fail.Setup(failCoins, _uiRoot);
            _fail.Show();
        }

        [Group("FailBtns"), Button("Hide Fail")]
        private void Btn_HideFail()
        {
            _fail?.Hide();
        }

        [Title("Settings & Retry")]
        [Group("SetBtns"), Button("Show Settings")]
        private void Btn_ShowSettings()
        {
            _settings?.Show();
        }

        [Group("SetBtns"), Button("Hide Settings")]
        private void Btn_HideSettings()
        {
            _settings?.Hide();
        }

        [Group("RetryBtns"), Button("Show Retry")]
        private void Btn_ShowRetry()
        {
            _retry?.Show();
        }

        [Group("RetryBtns"), Button("Hide Retry")]
        private void Btn_HideRetry()
        {
            _retry?.Hide();
        }

        [Title("Global")]
        [Button("Hide All")]
        private void Btn_HideAll()
        {
            _hud?.Hide();
            _win?.Hide();
            _fail?.Hide();
            _settings?.Hide();
            _retry?.Hide();
        }

        private (int stars, int winCoins, int failCoins) ComputeValues()
        {
            int stars;
            int wCoins;
            int fCoins;

            if (_forceStars)
            {
                stars = _forcedStars;
                wCoins = _manualWinCoins;
                fCoins = _manualFailCoins;
            }
            else if (_useActiveLevelConfig && LevelManager.Instance != null && LevelManager.Instance.ActiveLevelInstance != null)
            {
                LevelConfig cfg = LevelManager.Instance.ActiveLevelInstance.GetComponent<LevelConfig>();
                stars = StarEvaluator.EvaluateStars(cfg, _completionTimeSec);
                wCoins = cfg != null ? cfg.WinCoins : _manualWinCoins;
                fCoins = cfg != null ? cfg.FailCoins : _manualFailCoins;
            }
            else
            {
                stars = StarEvaluator.EvaluateStars(_manualTimeThresholdsSec, _completionTimeSec);
                wCoins = _manualWinCoins;
                fCoins = _manualFailCoins;
            }

            return (stars, wCoins, fCoins);
        }

        

        private CurrencyWallet GetWallet()
        {
            if (_wallet != null) return _wallet;
			var found = FindFirstObjectByType<CurrencyWallet>(FindObjectsInactive.Include);
            _wallet = found;
            return _wallet;
        }
    }
}


