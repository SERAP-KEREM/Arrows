using System.Collections;
using UnityEngine;
using TriInspector;
using SerapKeremGameKit._Singletons;
using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Logging;


namespace SerapKeremGameKit._Managers
{
    [HideMonoScript]
    [DefaultExecutionOrder(-1)]
    public class StateManager : MonoSingleton<StateManager>
    {
        [Title("Game State")]
        [SerializeField, ReadOnly] private GameState _currentState = GameState.None;

        [ShowInInspector, ReadOnly]
        public GameState CurrentState => _currentState;

        private Coroutine _timerCoroutine;
        private double _elapsedSeconds;
        private static readonly WaitForSeconds WaitOneSecond = new(1f);

        #region State Transitions
        public void SetLoading()
        {
            _currentState = GameState.Loading;
        }

        public void SetOnStart()
        {
            TraceLogger.Log("Level Started");
            _currentState = GameState.OnStart;
            StartTimer();
            // Example: resume gameplay time if paused
            // TimeManager.Instance.Resume();
        }

        public void SetOnWin()
        {
            TraceLogger.Log("Level Won");
            StopTimer();
            _currentState = GameState.OnWin;
            // Example: pause gameplay time for win panel
            // TimeManager.Instance.Pause();
        }

        public void SetOnLose()
        {
            TraceLogger.Log("Level Lost");
            StopTimer();
            _currentState = GameState.OnLose;
            // Example: pause gameplay time for lose panel
            // TimeManager.Instance.Pause();
        }

        public void SetOnRestart()
        {
            TraceLogger.Log("Level Restarted");
            SetOnLose();
        }
        #endregion

        #region Timer
        private void StartTimer()
        {
            StopTimer();
            _elapsedSeconds = 0;
            _timerCoroutine = StartCoroutine(TimerRoutine());
        }

        private void StopTimer()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        private IEnumerator TimerRoutine()
        {
            while (true)
            {
                yield return null;
                _elapsedSeconds += Time.unscaledDeltaTime;
            }
        }
        #endregion

        public double GetLevelTime() => _elapsedSeconds;

        public void ResetTimer()
        {
            _elapsedSeconds = 0;
        }
    }
}