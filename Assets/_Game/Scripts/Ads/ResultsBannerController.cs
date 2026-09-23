using ArrowNook.Puzzles;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

namespace ArrowNook.Ads
{
    public sealed class ResultsBannerController : MonoBehaviour
    {
        public static ResultsBannerController Instance { get; private set; }
        public bool PrivacyOptionsRequired => _consentChecked &&
            ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required;
        public bool PrivacyBusy => _consentBusy;

        private BannerView _banner;
        private RectTransform _results;
        private Vector2 _originalMin, _originalMax;
        private Rect _safeArea;
        private int _screenWidth, _screenHeight;
        private bool _loaded, _visible, _initialized, _initializing, _consentChecked, _consentBusy;
        private bool _paused, _focused = true;
        private float _sessionStart;
        private double _retryAt;
        private int _failures;

        private void Awake()
        {
            Instance = this;
            _sessionStart = Time.realtimeSinceStartup;
        }

        public void BeginResults(RectTransform panel, int level)
        {
            EndResults();
            if (!Application.isMobilePlatform || Application.isEditor || panel == null) return;
            float scale = DeviceScale;
            if (!BannerPolicy.IsEligible(level, Time.realtimeSinceStartup - _sessionStart,
                Screen.safeArea.width / scale)) return;
            _results = panel;
            _originalMin = panel.offsetMin;
            _originalMax = panel.offsetMax;
            ReserveFooter();
            if (!_consentChecked && !_consentBusy) GatherConsent();
            TryLoad();
        }

        public void EndResults()
        {
            SetVisible(false);
            if (_results != null)
            {
                _results.offsetMin = _originalMin;
                _results.offsetMax = _originalMax;
                _results = null;
            }
        }

        private static float DeviceScale => Mathf.Max(1f, MobileAds.Utils.GetDeviceScale());

        private void ReserveFooter()
        {
            if (_results == null) return;
            _safeArea = Screen.safeArea;
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            Canvas canvas = _results.GetComponentInParent<Canvas>();
            float uiScale = canvas != null ? canvas.scaleFactor : 1f;
            float footer = (BannerPolicy.HeightPoints + BannerPolicy.SeparationPoints) * DeviceScale;
            _results.offsetMin = _originalMin + new Vector2(_safeArea.xMin, _safeArea.yMin + footer) / uiScale;
            _results.offsetMax = _originalMax - new Vector2(Screen.width - _safeArea.xMax,
                Screen.height - _safeArea.yMax) / uiScale;
            PositionBanner();
        }

        private void PositionBanner()
        {
            if (_banner == null) return;
            float scale = DeviceScale;
            int x = Mathf.RoundToInt((_safeArea.center.x - BannerPolicy.WidthPoints * scale / 2f) / scale);
            int y = Mathf.RoundToInt((Screen.height - _safeArea.yMin - BannerPolicy.HeightPoints * scale) / scale);
            _banner.SetPosition(x, y);
        }

        private void Update()
        {
            if (_results != null && (_safeArea != Screen.safeArea || _screenWidth != Screen.width ||
                _screenHeight != Screen.height)) ReserveFooter();
            bool mayShow = _results != null && _results.gameObject.activeInHierarchy &&
                !_paused && _focused && !_consentBusy && ConsentInformation.CanRequestAds();
            SetVisible(mayShow && _loaded);
            if (mayShow) TryLoad();
        }

        private void GatherConsent()
        {
            _consentBusy = true;
            var request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = AdMobIds.TagForUnderAgeOfConsent
            };
            ConsentInformation.Update(request, error => OnMainThread(() =>
            {
                if (error != null) { FinishConsent(); return; }
                ConsentForm.LoadAndShowConsentFormIfRequired(_ => OnMainThread(FinishConsent));
            }));
        }

        private void FinishConsent()
        {
            _consentBusy = false;
            _consentChecked = true;
            if (!ConsentInformation.CanRequestAds()) return;
            if (_initialized) { TryLoad(); return; }
            if (_initializing) return;
            _initializing = true;
            MobileAds.SetRequestConfiguration(new RequestConfiguration
            {
                MaxAdContentRating = MaxAdContentRating.G,
                TagForChildDirectedTreatment = TagForChildDirectedTreatment.False,
                TagForUnderAgeOfConsent = AdMobIds.TagForUnderAgeOfConsent
                    ? TagForUnderAgeOfConsent.True
                    : TagForUnderAgeOfConsent.False
            });
            MobileAds.Initialize(_ => OnMainThread(() =>
            {
                _initialized = true;
                _initializing = false;
                TryLoad();
            }));
        }

        private void TryLoad()
        {
            if (!_initialized || _results == null || _paused || !_focused || _consentBusy ||
                !ConsentInformation.CanRequestAds() || _banner != null ||
                Time.realtimeSinceStartupAsDouble < _retryAt) return;
            string unitId = AdMobIds.BannerUnitId;
            if (string.IsNullOrWhiteSpace(unitId)) return;
            var banner = new BannerView(unitId, AdSize.Banner, AdPosition.Bottom);
            _banner = banner;
            banner.Hide();
            PositionBanner();
            banner.OnBannerAdLoaded += () => OnMainThread(() =>
            {
                if (_banner != banner) return;
                _loaded = true;
                _failures = 0;
                // Update decides visibility, including when loading completed after leaving this screen.
                if (_results == null || _paused || !_focused || _consentBusy) banner.Hide();
            });
            banner.OnBannerAdLoadFailed += _ => OnMainThread(() =>
            {
                if (_banner != banner) return;
                DestroyBanner();
                _retryAt = Time.realtimeSinceStartupAsDouble + BannerPolicy.RetryDelaySeconds(++_failures);
            });
            var request = new AdRequest();
            if (AdMobIds.ForceNonPersonalizedAds) request.Extras.Add("npa", "1");
            banner.LoadAd(request);
        }

        public void ShowPrivacyOptions(System.Action<string> completed)
        {
            if (_consentBusy || !PrivacyOptionsRequired) return;
            _consentBusy = true;
            DestroyBanner();
            ConsentForm.ShowPrivacyOptionsForm(error => OnMainThread(() =>
            {
                FinishConsent();
                completed?.Invoke(error == null ? null : "Privacy choices are unavailable. Please try again later.");
            }));
        }

        private void SetVisible(bool visible)
        {
            if (_visible == visible) return;
            _visible = visible;
            if (_banner == null) return;
            if (visible) _banner.Show(); else _banner.Hide();
        }

        private void DestroyBanner()
        {
            _loaded = false;
            _visible = false;
            BannerView previous = _banner;
            _banner = null;
            previous?.Destroy();
        }

        private void OnMainThread(System.Action action) => MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if (this != null) action();
        });

        private void OnApplicationPause(bool paused) { _paused = paused; if (paused) SetVisible(false); }
        private void OnApplicationFocus(bool focused) { _focused = focused; if (!focused) SetVisible(false); }
        private void OnDestroy()
        {
            EndResults();
            DestroyBanner();
            if (Instance == this) Instance = null;
        }
    }
}
