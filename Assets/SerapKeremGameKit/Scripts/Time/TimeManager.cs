using SerapKeremGameKit._Logging;
using SerapKeremGameKit._Singletons;
using System.Collections;
using UnityEngine;
using SerapKeremGameKit._UI;

namespace SerapKeremGameKit._Time
{
    public sealed class TimeManager : MonoSingleton<TimeManager>
    {
        [SerializeField] private int _targetFrameRate = 60;
        [SerializeField, Range(0f, 2f)] private float _defaultTimeScale = 1f;
        [SerializeField] private float _defaultFixedDeltaTime = 0.02f; // 50 Hz

        public bool IsPaused { get; private set; }

        private float _remainingTime;
        private Coroutine _countdownCoroutine;
        private System.Action<float> _onTimeUpdated;
        private System.Action _onTimeExpired;
        private UIRootController _uiRootController;

        public float RemainingTime => _remainingTime;
        public bool IsTimerRunning => _countdownCoroutine != null;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            Application.targetFrameRate = _targetFrameRate;
            SetFixedDeltaTime(_defaultFixedDeltaTime);
            ResetTimeScale();
            InitializeUIReference();
        }

        private void InitializeUIReference()
        {
            _uiRootController = FindFirstObjectByType<UIRootController>();
        }

        public void SetTargetFrameRate(int fps)
        {
            if (fps < 15) fps = 15;
            Application.targetFrameRate = fps;
        }

        public void SetTimeScale(float scale)
        {
            float clamped = Mathf.Max(0f, scale);
            Time.timeScale = clamped;
            IsPaused = clamped == 0f;
        }

        public void ResetTimeScale()
        {
            SetTimeScale(_defaultTimeScale);
        }

        public void Pause()
        {
            SetTimeScale(0f);
        }

        public void Resume()
        {
            ResetTimeScale();
        }

        public void SetFixedDeltaTime(float seconds)
        {
            if (seconds <= 0f) seconds = 0.02f;
            Time.fixedDeltaTime = seconds;
        }

        public float ScaledDeltaTime()
        {
            return Time.deltaTime;
        }

        public float UnscaledDeltaTime()
        {
            return Time.unscaledDeltaTime;
        }

        public IEnumerator WaitSeconds(float seconds)
        {
            if (seconds <= 0f) yield break;
            yield return new WaitForSeconds(seconds);
        }

        public IEnumerator WaitSecondsUnscaled(float seconds)
        {
            if (seconds <= 0f) yield break;
            yield return new WaitForSecondsRealtime(seconds);
        }

        public void StartCountdown(float duration, System.Action<float> onTimeUpdated = null, System.Action onTimeExpired = null)
        {
            StopCountdown();

            _remainingTime = duration;
            _onTimeUpdated = onTimeUpdated;
            _onTimeExpired = onTimeExpired;

            UpdateTimeDisplay(_remainingTime);
            _countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }

        private void UpdateTimeDisplay(float remainingTime)
        {
            if (_uiRootController == null)
            {
                InitializeUIReference();
            }

            if (_uiRootController != null)
            {
                _uiRootController.UpdateTimeDisplay(remainingTime);
            }
        }

        public void StopCountdown()
        {
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = null;
            }

            _onTimeUpdated = null;
            _onTimeExpired = null;
        }

        private IEnumerator CountdownCoroutine()
        {
            while (_remainingTime > 0f)
            {
                yield return new WaitForSeconds(1f);
                _remainingTime -= 1f;

                if (_remainingTime < 0f)
                {
                    _remainingTime = 0f;
                }

                UpdateTimeDisplay(_remainingTime);
                _onTimeUpdated?.Invoke(_remainingTime);

                if (_remainingTime <= 0f)
                {
                    _onTimeExpired?.Invoke();
                    break;
                }
            }

            _countdownCoroutine = null;
        }
    }
}


