using System;
using UnityEngine;

namespace ArrowNook.Ads
{
    public static class AdMobIds
    {
        public const string ConfigResourcePath = "Secrets/AdMobIds";
        public const string AndroidTestAppId = "ca-app-pub-3940256099942544~3347511713";
        public const string AndroidTestBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
        public const string AndroidTestInterstitialUnitId = "ca-app-pub-3940256099942544/1033173712";
        public const string IosTestAppId = "ca-app-pub-3940256099942544~1458002511";
        public const string IosTestBannerUnitId = "ca-app-pub-3940256099942544/2934735716";
        public const string IosTestInterstitialUnitId = "ca-app-pub-3940256099942544/4411468910";

        private static Config _config;
        private static bool _loaded;

        public static string AndroidAppId => Load().androidAppId;
        public static string AndroidBannerUnitId => Resolve(Load().androidBannerUnitId, AndroidTestBannerUnitId);
        public static string AndroidInterstitialUnitId => Resolve(Load().androidInterstitialUnitId, AndroidTestInterstitialUnitId);
        public static string IosAppId => Load().iosAppId;
        public static string IosBannerUnitId => Resolve(Load().iosBannerUnitId, IosTestBannerUnitId);
        public static string IosInterstitialUnitId => Resolve(Load().iosInterstitialUnitId, IosTestInterstitialUnitId);
        public static bool ForceNonPersonalizedAds => Load().forceNonPersonalizedAds;
        public static bool TagForUnderAgeOfConsent => Load().tagForUnderAgeOfConsent;

#if UNITY_ANDROID
        public static string BannerUnitId => AndroidBannerUnitId;
        public static string InterstitialUnitId => AndroidInterstitialUnitId;
#elif UNITY_IOS
        public static string BannerUnitId => IosBannerUnitId;
        public static string InterstitialUnitId => IosInterstitialUnitId;
#else
        public static string BannerUnitId => string.Empty;
        public static string InterstitialUnitId => string.Empty;
#endif

        public static bool UsesOfficialTestIds =>
            IsOfficialTestId(AndroidAppId) ||
            IsOfficialTestId(AndroidBannerUnitId) ||
            IsOfficialTestId(AndroidInterstitialUnitId) ||
            IsOfficialTestId(IosAppId) ||
            IsOfficialTestId(IosBannerUnitId) ||
            IsOfficialTestId(IosInterstitialUnitId);

        public static bool HasProductionAndroidIds =>
            IsRealAdMobAppId(AndroidAppId) &&
            IsRealAdMobUnitId(Load().androidBannerUnitId) &&
            IsRealAdMobUnitId(Load().androidInterstitialUnitId) &&
            !IsOfficialTestId(AndroidAppId) &&
            !IsOfficialTestId(Load().androidBannerUnitId) &&
            !IsOfficialTestId(Load().androidInterstitialUnitId);

        public static bool IsRealAdMobAppId(string value) =>
            !string.IsNullOrWhiteSpace(value) &&
            value.StartsWith("ca-app-pub-", StringComparison.Ordinal) &&
            value.Contains("~");

        public static bool IsRealAdMobUnitId(string value) =>
            !string.IsNullOrWhiteSpace(value) &&
            value.StartsWith("ca-app-pub-", StringComparison.Ordinal) &&
            value.Contains("/");

        public static bool IsOfficialTestId(string value) =>
            !string.IsNullOrWhiteSpace(value) &&
            value.IndexOf("3940256099942544", StringComparison.Ordinal) >= 0;

        private static string Resolve(string productionValue, string testValue)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return testValue;
#else
            return productionValue;
#endif
        }

        private static Config Load()
        {
            if (_loaded) return _config;
            _loaded = true;
            _config = new Config();
            TextAsset asset = Resources.Load<TextAsset>(ConfigResourcePath);
            if (asset == null) return _config;
            try
            {
                JsonUtility.FromJsonOverwrite(asset.text, _config);
            }
            catch (Exception error)
            {
                Debug.LogError($"Invalid AdMobIds config: {error.Message}");
            }

            return _config;
        }

        [Serializable]
        private sealed class Config
        {
            public string androidAppId = "";
            public string androidBannerUnitId = "";
            public string androidInterstitialUnitId = "";
            public string iosAppId = "";
            public string iosBannerUnitId = "";
            public string iosInterstitialUnitId = "";
            public bool forceNonPersonalizedAds = true;
            public bool tagForUnderAgeOfConsent = true;
        }
    }
}
