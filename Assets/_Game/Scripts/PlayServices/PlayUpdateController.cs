using System.Collections;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using Google.Play.AppUpdate;
#endif

namespace ArrowNook.PlayServices
{
    /// <summary>
    /// Checks Google Play for available updates once per session and lets Play own the update UI.
    /// </summary>
    public sealed class PlayUpdateController : MonoBehaviour
    {
        private const int FlexibleUpdatePriority = 2;
        private const int ImmediateUpdatePriority = 4;
        private const int FlexibleStalenessDays = 2;
        private const int ImmediateStalenessDays = 7;

        private bool _checking;

#if UNITY_ANDROID && !UNITY_EDITOR
        private AppUpdateManager _updateManager;
#endif

        private void Start()
        {
            TryCheckForUpdates();
        }

        public void TryCheckForUpdates()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_checking || !Application.isMobilePlatform)
                return;

            StartCoroutine(CheckForUpdate());
#endif
        }

        private IEnumerator CheckForUpdate()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _checking = true;

            if (_updateManager == null)
                _updateManager = new AppUpdateManager();

            var infoOperation = _updateManager.GetAppUpdateInfo();
            yield return infoOperation;

            if (!infoOperation.IsSuccessful)
            {
                _checking = false;
                yield break;
            }

            AppUpdateInfo appUpdateInfo = infoOperation.GetResult();
            if (appUpdateInfo == null)
            {
                _checking = false;
                yield break;
            }

            if (appUpdateInfo.AppUpdateStatus == AppUpdateStatus.Downloaded)
            {
                yield return CompleteFlexibleUpdate();
                _checking = false;
                yield break;
            }

            AppUpdateOptions immediateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
            AppUpdateOptions flexibleOptions = AppUpdateOptions.FlexibleAppUpdateOptions();

            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
            {
                if (appUpdateInfo.IsUpdateTypeAllowed(immediateOptions))
                    yield return StartUpdate(appUpdateInfo, immediateOptions, false);

                _checking = false;
                yield break;
            }

            if (appUpdateInfo.UpdateAvailability != UpdateAvailability.UpdateAvailable)
            {
                _checking = false;
                yield break;
            }

            int stalenessDays = Mathf.Max(0, appUpdateInfo.ClientVersionStalenessDays);
            bool wantsImmediate = appUpdateInfo.UpdatePriority >= ImmediateUpdatePriority ||
                stalenessDays >= ImmediateStalenessDays;
            bool wantsFlexible = appUpdateInfo.UpdatePriority >= FlexibleUpdatePriority ||
                stalenessDays >= FlexibleStalenessDays;

            if (wantsImmediate && appUpdateInfo.IsUpdateTypeAllowed(immediateOptions))
            {
                yield return StartUpdate(appUpdateInfo, immediateOptions, false);
            }
            else if (wantsFlexible && appUpdateInfo.IsUpdateTypeAllowed(flexibleOptions))
            {
                yield return StartUpdate(appUpdateInfo, flexibleOptions, true);
            }

            _checking = false;
#else
            yield break;
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private IEnumerator StartUpdate(AppUpdateInfo appUpdateInfo, AppUpdateOptions options, bool completeWhenDownloaded)
        {
            AppUpdateRequest request = _updateManager.StartUpdate(appUpdateInfo, options);
            while (!request.IsDone)
                yield return null;

            if (!completeWhenDownloaded || request.Error != AppUpdateErrorCode.NoError ||
                request.Status != AppUpdateStatus.Downloaded)
                yield break;

            yield return CompleteFlexibleUpdate();
        }

        private IEnumerator CompleteFlexibleUpdate()
        {
            var completeOperation = _updateManager.CompleteUpdate();
            yield return completeOperation;
        }
#endif
    }
}
