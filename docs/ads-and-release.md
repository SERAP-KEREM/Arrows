# ArrowNook: Logic Puzzle — Ads & Release Checklist

## Before enabling production ads and submitting to Google Play

This document lists **every step** required before the release build guard
(`ArrowNookReleaseGuard.cs`) will permit a non-development Android build.

---

## 1. AdMob account setup

- [ ] Sign into [AdMob](https://admob.google.com/) with your Google account.
- [ ] Create an app entry for **ArrowNook: Logic Puzzle** (Android).
- [ ] Copy the real **Android App ID** (format `ca-app-pub-XXXXXXXXXXXXXXXX~NNNNNNNNNN`).

## 2. Replace test IDs in Unity

| File | Field | Replace with |
|---|---|---|
| `Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset` | `adMobAndroidAppId` | Your real Android App ID |
| `Assets/_Game/Resources/Secrets/AdMobIds.json` | `androidAppId` | Your real Android App ID |
| `Assets/_Game/Resources/Secrets/AdMobIds.json` | `androidBannerUnitId` | Your real banner ad unit ID |
| `Assets/_Game/Resources/Secrets/AdMobIds.json` | `androidInterstitialUnitId` | Your real interstitial ad unit ID |

> **Never commit real Ad IDs to a public repo.**
> `Assets/_Game/Resources/Secrets/AdMobIds.json` is ignored by git. Start from
> `docs/admob-ids.template.json`, then create the local file in Unity.
> Editor and Development Build players always use Google's official test units;
> non-development release builds use the local production IDs.

## 3. Privacy policy

- [ ] Write a privacy policy covering:
  - Data collected by Google Mobile Ads (device ID, IP, ad interactions).
  - Data stored locally (level progress, settings).
  - Contact information (your name and email).
- [ ] Host it at a permanent public URL (e.g. GitHub Pages, Notion, or Google Sites).
- [ ] Add the URL to the Play Console "App content" → "Privacy policy" field.
- [ ] Update the hosted version from `docs/privacy-policy.md`.
- [ ] Keep `Assets/_Game/Resources/Branding/PrivacyNotice.txt` aligned with the hosted policy.

## 4. Age / audience declaration (Play Console)

- [ ] In Play Console → **App content** → **Target audience and content**, set:
  - Target age: **13 and over**.
  - Confirm the game does not target children under 13.
- [ ] Complete the **Ads declaration**:
  - Contains ads: **Yes**.
  - Ad type: Banner + interstitial.
  - Ad SDK: Google AdMob.
- [ ] Complete the **Data safety** section for all SDKs (AdMob collects data).

## 5. Consent / UMP configuration

The consent flow is already wired (`GatherConsent` in `ResultsBannerController`).
Before release:
- [ ] In the AdMob console → **Privacy & messaging**, create a GDPR message and link it.
- [ ] Set `TagForUnderAgeOfConsent = False` in `FinishConsent()` once you have verified the
  audience is 13+ and your legal review is complete.

## 6. Google Play review and update flows

The project includes Google's Play In-App Review and In-App Updates Unity packages:

- `com.google.play.review` prompts through the native Play review sheet after a natural
  level-complete transition, starting at level 5, at most once per app version and no more
  than once every 60 days. A review attempt skips the interstitial for that same transition.
- `com.google.play.appupdate` checks once per app session. Medium-priority or stale updates
  use flexible update flow; high-priority or week-old updates use immediate flow when Google
  Play allows it.

Before release:

- [ ] Open Unity once and let Package Manager resolve `Packages/manifest.json`.
- [ ] Test review and update behavior from a Play internal testing track; these flows do not
  behave like production when the app is side-loaded.
- [ ] If you want forced critical updates, set `inAppUpdatePriority` through the Google Play
  Developer API when creating the release.

## 7. Run release validation

Use **Tools > Arrow Game > Validate Play Store Release**. The release guard also runs
automatically for non-development Android builds and blocks builds that still use
test ads, lack signing, lack ARM64, lack an App Bundle setting, lack the Google Play
review/update packages, or contain development privacy copy.

## 8. Final build checklist

- [ ] Increment `PlayerSettings.bundleVersionCode`.
- [ ] Assign Android launcher icons in Player Settings.
- [ ] Enable **Build App Bundle**.
- [ ] Configure a custom upload keystore.
- [ ] Set **Build type = Release** (disable Development Build toggle).
- [ ] Run on a physical Android device and verify:
  - A real banner appears on the results screen after level 4.
  - A real interstitial appears after completing 3 levels (warmup timer applies).
  - Privacy-choices button appears in Settings for EEA users.
  - In-app update prompt appears from Play internal testing when a higher version is available.
  - In-app review request does not block level progression if Play declines to show it.
  - "Made with Unity" logo displays in Settings > About.
  - No original SERAP-KEREM branding is visible to end users.

## 9. Play Store submission

- [ ] Upload signed AAB to an Internal testing track first.
- [ ] Verify rating: set **ESRB E** (Everyone) / **PEGI 3** (no violence, no adult content).
- [ ] Fill in store listing: use the name **ArrowNook: Logic Puzzle**.
- [ ] Add screenshots (portrait, minimum 2 phones + 1 tablet).
- [ ] Promote to Production once internal testing passes.

---

_Developer: Anupam Pradhan, India — solo developer_
