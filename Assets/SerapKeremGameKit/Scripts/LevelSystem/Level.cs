using SerapKeremGameKit._Camera;
using SerapKeremGameKit._InputSystem;
using SerapKeremGameKit._Logging;
using SerapKeremGameKit._Managers;
using System.Collections;
using TriInspector;
using UnityEngine;
using Array2DEditor;
using _Game.Line;
using _Game.UI;



namespace SerapKeremGameKit._LevelSystem
{
    public class Level : MonoBehaviour
    {

        // [Title("Coins Settings")] // coin settings can be added here if level-specific

        [Title("Grid Settings"), PropertyOrder(2)]
        [SerializeField] private Array2DInt _tileSizeArray;

        [Title("Time Settings")]
        [SerializeField, Min(0f)] private float _levelTime = 120f;
        public float LevelTime => _levelTime;

        [ReadOnly]
        [SerializeField] private bool _isLevelWon;

        [Title("Money Settings")]
        [SerializeField] private long _money = 10;
        public long Money => _money;


        private Coroutine _winCoroutine;
        private Coroutine _loseCoroutine;

        [SerializeField] private LineManager _lineManager;
        public LineManager LineManager { get => _lineManager; set => _lineManager = value; }
        // [SerializeField] private Transform _levelCameraPoint;

        // [SerializeField] private float _currentLevelSize = 0f;
        public virtual void Load()
        {
            gameObject.SetActive(true);
            _isLevelWon = false;
            if (_winCoroutine != null) { StopCoroutine(_winCoroutine); _winCoroutine = null; }
            if (_loseCoroutine != null) { StopCoroutine(_loseCoroutine); _loseCoroutine = null; }
            Initialize();
        }
        private void Initialize()
        {
            StartCoroutine(InitializeCoroutine());

        }
        private IEnumerator InitializeCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            InitializeCamera();
            InitializeLines();
        }

        private void InitializeLines()
        {
            if (LevelManager.Instance.ActiveLevelInstance.LineManager)
            {
                LevelManager.Instance.ActiveLevelInstance.LineManager.InitializeLines(transform);
            }
            else
            {
                TraceLogger.LogWarning("LineManager is not initialized. Lines will not be initialized.", this);
            }
        }

        private void InitializeCamera()
        {
            if (CameraManager.Instance == null)
            {
                TraceLogger.LogError("CameraManager.Instance is null! Cannot initialize camera position.");
                return;
            }

            //InitializeCameraPosition 
            //if (_levelCameraCampoint != null)
            //{
            //    CameraManager.Instance.InitializeCameraPosition(_levelCameraCampoint);
            //    RichLogger.Log($"Level Loaded: Camera positioned to {_levelCameraCampoint.name} in {gameObject.name}");
            //}
            //else
            //{
            //    RichLogger.LogWarning($"Level Load Warning: '_levelCameraCampoint' is not assigned for {gameObject.name}. Camera position might be incorrect.");
            //}

            //AdjustCameraByLevelSize
            //if (_currentLevelSize != 0)
            //{
            //    CameraManager.Instance.AdjustCameraByLevelSize(_currentLevelSize); 
            //    RichLogger.Log($"Level Loaded: Camera adjusted by level size: {_currentLevelSize} in {gameObject.name}");
            //}
            //else
            //{
            //    Debug.LogError("Level Load Error: CameraManager instance is not found! Ensure it's present and initialized.");
            //}
        }

        public virtual void Play()
        {
            if (InputHandler.Instance != null)
            {
                InputHandler.Instance.UnlockInput();
            }

            if (LivesManager.IsInitialized)
            {
                LivesManager.Instance.ResetLives();
                LivesManager.Instance.OnLivesDepleted += HandleLivesDepleted;
            }

            if (_lineManager != null)
            {
                _lineManager.OnAllLinesRemoved += HandleAllLinesRemoved;
            }
        }

        private void HandleLivesDepleted()
        {
            CheckLoseCondition();
        }

        private void HandleAllLinesRemoved()
        {
            CheckWinCondition();
        }

        public void CheckWinCondition()
        {
            if (_isLevelWon) return;

            _isLevelWon = true;
            _winCoroutine = StartCoroutine(WinCoroutine());
        }

        private IEnumerator WinCoroutine()
        {
            if (InputHandler.Instance != null) InputHandler.Instance.LockInput();
            yield return new WaitForSeconds(0.5f);
            // Example: reward coins for win
            // int coins = PlayerPrefs.GetInt("skgk.currency.coins", 0) + 10; PlayerPrefs.SetInt("skgk.currency.coins", coins); PlayerPrefs.Save();
            LevelManager.Instance.Win();
        }

        public void CheckLoseCondition()
        {
            if (_loseCoroutine != null) return;

            _loseCoroutine = StartCoroutine(LoseCoroutine());
        }

        private IEnumerator LoseCoroutine()
        {
            if (InputHandler.Instance != null) InputHandler.Instance.LockInput();
            yield return new WaitForSeconds(0.5f);

            LevelManager.Instance.Lose();
        }

        private void OnDestroy()
        {
            if (LivesManager.IsInitialized)
            {
                LivesManager.Instance.OnLivesDepleted -= HandleLivesDepleted;
            }

            if (_lineManager != null)
            {
                _lineManager.OnAllLinesRemoved -= HandleAllLinesRemoved;
            }
        }
    }
}