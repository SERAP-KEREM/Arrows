using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using Google.Play.Review;
#endif

namespace ArrowNook.PlayServices
{
    /// <summary>
    /// Requests the native Google Play review sheet at calm level-transition moments.
    /// Google Play may silently decline to show a dialog, so game flow must continue either way.
    /// </summary>
    public sealed class PlayReviewController : MonoBehaviour
    {
        private const int MinimumCompletedLevel = 5;
        private const int MinimumDaysBetweenAttempts = 60;
        private const float RequestTimeoutSeconds = 4f;
        private const string PromptedVersionKey = "arrownook.review.prompted_version";
        private const string LastAttemptTicksKey = "arrownook.review.last_attempt_ticks";

        private bool _busy;

#if UNITY_ANDROID && !UNITY_EDITOR
        private ReviewManager _reviewManager;
#endif

        public bool TryRequestReview(int completedLevelNumber, Action completed)
        {
            if (!CanRequestReview(completedLevelNumber))
                return false;

            StartCoroutine(RequestReview(completed));
            return true;
        }

        private bool CanRequestReview(int completedLevelNumber)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_busy || !Application.isMobilePlatform) return false;
            if (completedLevelNumber < MinimumCompletedLevel) return false;
            if (PlayerPrefs.GetString(PromptedVersionKey, string.Empty) == Application.version) return false;

            string rawTicks = PlayerPrefs.GetString(LastAttemptTicksKey, string.Empty);
            if (long.TryParse(rawTicks, NumberStyles.Integer, CultureInfo.InvariantCulture, out long ticks) &&
                ticks > 0 && ticks <= DateTime.MaxValue.Ticks)
            {
                DateTime lastAttempt = new DateTime(ticks, DateTimeKind.Utc);
                if ((DateTime.UtcNow - lastAttempt).TotalDays < MinimumDaysBetweenAttempts)
                    return false;
            }

            return true;
#else
            return false;
#endif
        }

        private IEnumerator RequestReview(Action completed)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _busy = true;
            MarkAttempted();

            if (_reviewManager == null)
                _reviewManager = new ReviewManager();

            var requestFlow = _reviewManager.RequestReviewFlow();
            float requestDeadline = Time.realtimeSinceStartup + RequestTimeoutSeconds;
            while (!requestFlow.IsDone && Time.realtimeSinceStartup < requestDeadline)
                yield return null;

            if (!requestFlow.IsDone)
            {
                _busy = false;
                completed?.Invoke();
                yield break;
            }

            if (requestFlow.Error == ReviewErrorCode.NoError)
            {
                PlayReviewInfo reviewInfo = requestFlow.GetResult();
                var launchFlow = _reviewManager.LaunchReviewFlow(reviewInfo);
                yield return launchFlow;
            }

            _busy = false;
#endif
            completed?.Invoke();
            yield break;
        }

        private static void MarkAttempted()
        {
            PlayerPrefs.SetString(PromptedVersionKey, Application.version);
            PlayerPrefs.SetString(LastAttemptTicksKey, DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.Save();
        }
    }
}
