using System;
using System.Collections.Generic;
using System.IO;
using ArrowNook.Ads;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ArrowNook.Editor
{
    public sealed class ArrowNookReleaseGuard : IPreprocessBuildWithReport
    {
        private const string GoogleAdsSettingsPath = "Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset";
        private const string PackageManifestPath = "Packages/manifest.json";
        private const string AdMobIdsPath = "Assets/_Game/Resources/Secrets/AdMobIds.json";
        private const string PrivacyNoticePath = "Assets/_Game/Resources/Branding/PrivacyNotice.txt";
        private const string PlayIconPath = "Assets/GameImages/Branding/PlayStore/AppIcon_512.png";
        private const string FeatureGraphicPath = "Assets/GameImages/Branding/PlayStore/FeatureGraphic_1024x500.jpg";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android && report.summary.platform != BuildTarget.iOS)
                return;
            if ((report.summary.options & BuildOptions.Development) != 0) return;
            if (report.summary.platform == BuildTarget.iOS)
                throw new BuildFailedException("ArrowNook release validation is configured for Google Play only.");

            ValidatePlayStoreRelease();
        }

        [MenuItem("Tools/Arrow Game/Validate Play Store Release")]
        public static void ValidatePlayStoreRelease()
        {
            var errors = new List<string>();
            CheckAndroidPlayerSettings(errors);
            CheckSigning(errors);
            CheckPlayPackages(errors);
            CheckAdMobConfiguration(errors);
            CheckPrivacyNotice(errors);
            CheckStoreAssets(errors);

            if (errors.Count > 0)
                throw new BuildFailedException("ArrowNook is not ready for a Google Play release:\n- " +
                    string.Join("\n- ", errors));

            Debug.Log("ArrowNook Google Play release validation passed.");
        }

        private static void CheckAndroidPlayerSettings(List<string> errors)
        {
            string packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            if (string.IsNullOrWhiteSpace(packageName) || packageName == "com.DefaultCompany.ProductName")
                errors.Add("Set a permanent Android package name.");

            if ((int)PlayerSettings.Android.targetSdkVersion < 36)
                errors.Add("Android target SDK must be API 36 or higher.");

            if ((PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARM64) == 0)
                errors.Add("Enable ARM64 in Android target architectures.");

            if (!EditorUserBuildSettings.buildAppBundle)
                errors.Add("Enable Build App Bundle before creating the Play upload.");

            Texture2D[] icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Android);
            bool hasIcon = false;
            if (icons != null)
            {
                foreach (Texture2D icon in icons)
                {
                    if (icon == null) continue;
                    hasIcon = true;
                    break;
                }
            }

            if (!hasIcon)
                errors.Add("Assign Android launcher icons in Player Settings.");
        }

        private static void CheckSigning(List<string> errors)
        {
            if (!PlayerSettings.Android.useCustomKeystore)
                errors.Add("Configure a custom Android upload keystore.");
            if (string.IsNullOrWhiteSpace(PlayerSettings.Android.keystoreName))
                errors.Add("Set Android keystore name/path.");
            if (string.IsNullOrWhiteSpace(PlayerSettings.Android.keyaliasName))
                errors.Add("Set Android key alias.");
        }

        private static void CheckPlayPackages(List<string> errors)
        {
            if (!File.Exists(PackageManifestPath))
            {
                errors.Add("Unity package manifest is missing.");
                return;
            }

            string manifest = File.ReadAllText(PackageManifestPath);
            RequirePackage(errors, manifest, "com.google.play.review");
            RequirePackage(errors, manifest, "com.google.play.appupdate");
        }

        private static void RequirePackage(List<string> errors, string manifest, string packageName)
        {
            if (manifest.IndexOf($"\"{packageName}\"", StringComparison.Ordinal) < 0)
                errors.Add($"Add Google Play package '{packageName}' to {PackageManifestPath}.");
        }

        private static void CheckAdMobConfiguration(List<string> errors)
        {
            string appId = ReadSerializedString(GoogleAdsSettingsPath, "adMobAndroidAppId");
            if (!AdMobIds.IsRealAdMobAppId(appId) || AdMobIds.IsOfficialTestId(appId))
                errors.Add("Replace the Google Mobile Ads Android app ID with your production AdMob app ID.");

            if (!File.Exists(AdMobIdsPath))
            {
                errors.Add($"Create local production ad config: {AdMobIdsPath}.");
                return;
            }

            string json = File.ReadAllText(AdMobIdsPath);
            AdMobIdsFile ids;
            try
            {
                ids = JsonUtility.FromJson<AdMobIdsFile>(json);
            }
            catch (Exception error)
            {
                errors.Add($"{AdMobIdsPath} is not valid JSON: {error.Message}");
                return;
            }

            if (ids == null)
            {
                errors.Add($"{AdMobIdsPath} is not valid JSON.");
                return;
            }

            if (!AdMobIds.IsRealAdMobAppId(ids.androidAppId) || AdMobIds.IsOfficialTestId(ids.androidAppId))
                errors.Add("AdMobIds.json must contain a production androidAppId.");
            else if (!string.Equals(appId, ids.androidAppId, StringComparison.Ordinal))
                errors.Add("GoogleMobileAdsSettings.asset Android app ID must match AdMobIds.json androidAppId.");
            if (!AdMobIds.IsRealAdMobUnitId(ids.androidBannerUnitId) ||
                AdMobIds.IsOfficialTestId(ids.androidBannerUnitId))
                errors.Add("AdMobIds.json must contain a production androidBannerUnitId.");
            if (!AdMobIds.IsRealAdMobUnitId(ids.androidInterstitialUnitId) ||
                AdMobIds.IsOfficialTestId(ids.androidInterstitialUnitId))
                errors.Add("AdMobIds.json must contain a production androidInterstitialUnitId.");
        }

        private static void CheckPrivacyNotice(List<string> errors)
        {
            if (!File.Exists(PrivacyNoticePath))
            {
                errors.Add("Privacy notice resource is missing.");
                return;
            }

            string text = File.ReadAllText(PrivacyNoticePath);
            string[] blocked =
            {
                "Development privacy notice",
                "test build",
                "official test",
                "not a published Play Store privacy policy",
                "ACTION NEEDED",
                "TODO",
                "YOUR_",
                "example.com"
            };

            foreach (string phrase in blocked)
            {
                if (text.IndexOf(phrase, StringComparison.OrdinalIgnoreCase) >= 0)
                    errors.Add($"Replace development privacy copy before release: found '{phrase}'.");
            }
        }

        private static void CheckStoreAssets(List<string> errors)
        {
            CheckTexture(errors, PlayIconPath, 512, 512, "Play Store app icon");
            CheckTexture(errors, FeatureGraphicPath, 1024, 500, "Play Store feature graphic");
        }

        private static void CheckTexture(List<string> errors, string path, int width, int height, string label)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null)
            {
                errors.Add($"{label} is missing: {path}.");
                return;
            }

            if (texture.width != width || texture.height != height)
                errors.Add($"{label} must be {width}x{height}px.");
        }

        private static string ReadSerializedString(string path, string propertyName)
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (asset == null) return string.Empty;
            var serialized = new SerializedObject(asset);
            SerializedProperty property = serialized.FindProperty(propertyName);
            return property != null ? property.stringValue : string.Empty;
        }

        [Serializable]
        private sealed class AdMobIdsFile
        {
            public string androidAppId;
            public string androidBannerUnitId;
            public string androidInterstitialUnitId;
        }
    }
}
