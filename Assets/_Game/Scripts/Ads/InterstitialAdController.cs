using ArrowNook.Puzzles;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

namespace ArrowNook.Ads
{
    /// <summary>
    /// Shows a non-personalized interstitial ad at natural level-transition moments.
    /// Policy rules (see <see cref="BannerPolicy"/>):
    ///   • Never on the first <see cref="BannerPolicy.FirstAdLevel"/> levels.
    ///   • Never within the first <see cref="BannerPolicy.WarmupSeconds"/> of a session.
    ///   • Never more than once every <see cref="BannerPolicy.InterstitialCooldownSeconds"/> seconds.
    ///   • One ad every <see cref="BannerPolicy.InterstitialEveryNLevels"/> completed levels.
    ///
    /// Wire-up: place this component on the same persistent GameObject as
    /// <see cref="ResultsBannerController"/> and call <see cref="OnLevelComplete"/> from
    /// your win/level-advance code.
    /// </summary>
    public sealed class InterstitialAdController : MonoBehaviour
    {
        public static InterstitialAdController Instance { get; private set; }

        private InterstitialAd _interstitial;
        private float _sessionStart;
        private double _nextShowAt;   // realtime seconds – cooling-down between shows
        private int _levelsSinceLastAd;
        private bool _paused, _focused = true;
        private bool _showing;

        private void Awake()
        {
            Instance = this;
            _sessionStart = Time.realtimeSinceStartup;
        }

        // ── Public API ─────────────────────────────────────────────────────────

        /// <summary>
        /// Call this when the player finishes a level and the next level is about to load.
        /// The controller decides silently whether to show an interstitial.
        /// </summary>
        /// <param name="completedLevelNumber">The number of the level just beaten.</param>
        /// <param name="onDismissed">Callback fired after the ad closes (or immediately if
        ///   the ad was not shown). Use this to actually advance to the next level.</param>
        public void OnLevelComplete(int completedLevelNumber, System.Action onDismissed)
        {
            _levelsSinceLastAd++;

            bool eligible =
                completedLevelNumber >= BannerPolicy.FirstAdLevel &&
                (Time.realtimeSinceStartup - _sessionStart) >= BannerPolicy.WarmupSeconds &&
                Time.realtimeSinceStartupAsDouble >= _nextShowAt &&
                _levelsSinceLastAd >= BannerPolicy.InterstitialEveryNLevels &&
                !_paused && _focused &&
                Application.isMobilePlatform && !Application.isEditor;

            if (!eligible || _interstitial == null || !_interstitial.CanShowAd())
            {
                // Pre-load for the next time even if we're not showing now.
                EnsureLoaded();
                onDismissed?.Invoke();
                return;
            }

            _showing = true;
            _levelsSinceLastAd = 0;
            _nextShowAt = Time.realtimeSinceStartupAsDouble + BannerPolicy.InterstitialCooldownSeconds;

            _interstitial.OnAdFullScreenContentClosed += () => OnMainThread(() =>
            {
                _showing = false;
                DestroyInterstitial();
                EnsureLoaded();
                onDismissed?.Invoke();
            });

            _interstitial.OnAdFullScreenContentFailed += _ => OnMainThread(() =>
            {
                _showing = false;
                DestroyInterstitial();
                EnsureLoaded();
                onDismissed?.Invoke();
            });

            _interstitial.Show();
        }

        // ── Internal ───────────────────────────────────────────────────────────

        private void Start() => EnsureLoaded();

        private void EnsureLoaded()
        {
            if (_interstitial != null) return;
            if (!Application.isMobilePlatform || Application.isEditor) return;
            if (!ConsentInformation.CanRequestAds()) return;
            string unitId = AdMobIds.InterstitialUnitId;
            if (string.IsNullOrWhiteSpace(unitId)) return;
            var request = new AdRequest();
            if (AdMobIds.ForceNonPersonalizedAds) request.Extras.Add("npa", "1");
            InterstitialAd.Load(unitId, request, (ad, error) =>
            {
                if (error != null) return;
                OnMainThread(() => _interstitial = ad);
            });
        }

        private void DestroyInterstitial()
        {
            InterstitialAd previous = _interstitial;
            _interstitial = null;
            previous?.Destroy();
        }

        private void OnMainThread(System.Action action) =>
            MobileAdsEventExecutor.ExecuteInUpdate(() => { if (this != null) action(); });

        private void OnApplicationPause(bool paused) => _paused = paused;
        private void OnApplicationFocus(bool focused) => _focused = focused;

        private void OnDestroy()
        {
            DestroyInterstitial();
            if (Instance == this) Instance = null;
        }
    }
}
