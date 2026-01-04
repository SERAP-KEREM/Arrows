using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Singletons;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;
using SerapKeremGameKit._Logging;
using SerapKeremGameKit._Utilities;

namespace SerapKeremGameKit._Managers
{
    [DefaultExecutionOrder(-2)]
    public class LevelManager : MonoSingleton<LevelManager>
    {
        #region Properties & Data Access

        private const string ProgressKey = PreferencesKeys.ProgressData;
        public int ActiveLevelNumber
        {
            get => PlayerPrefs.GetInt(ProgressKey, 1);
            set { PlayerPrefs.SetInt(ProgressKey, value); SaveUtility.SaveImmediate(); }
        }

        [Tooltip("Use random selection after tutorials are completed.")]
        [SerializeField] private bool _useRandomSelection = true;

        [Title("Level Collections")]
        [ListDrawerSettings(Draggable = true, AlwaysExpanded = false)]
        [FormerlySerializedAs("_gameplayLevels")]
        [SerializeField, Required] private Level[] _levels;

        public Level ActiveLevelInstance { get; private set; }
        public int ProcessedLevelIndex { get; private set; }

        // Public accessors for external systems
        public Level[] GameplayLevels => _levels;
        public int GameplayLevelCount => _levels != null ? _levels.Length : 0;

        #endregion

        // Events removed to keep template simple and robust

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            PerformInitialValidation();
        }

        void Start()
        {
            StartCurrentLevelInstance();
        }

        public void StartCurrentLevelInstance()
        {
            ConfigureEnvironment();
            LoadCurrentLevel();
        }

        #endregion

        #region Core Level Management

        public void LoadCurrentLevel()
        {
            var selection = ComputeLevelSelection();
            ProcessedLevelIndex = selection.targetIndex;
            InstantiateAndBegin(selection.selectedLevel);
        }

        private (Level selectedLevel, int targetIndex) ComputeLevelSelection()
        {
            int currentProgress = ActiveLevelNumber;
            return ResolveGameplaySelection(currentProgress);
        }

        private (Level selectedLevel, int targetIndex) ResolveGameplaySelection(int adjustedProgress)
        {
            int totalGameplayLevels = _levels.Length;
            int calculatedIndex = ClampOrWrapIndex(adjustedProgress, totalGameplayLevels);

            return (_levels[calculatedIndex - 1], calculatedIndex);
        }

        private int ClampOrWrapIndex(int progressValue, int totalAvailable)
        {
            if (progressValue <= totalAvailable)
                return progressValue;

            if (_useRandomSelection)
                return GetRandomIndex(totalAvailable);

            return WrapIndex(progressValue, totalAvailable);
        }

        private int GetRandomIndex(int maxRange) => Random.Range(1, maxRange + 1);

        private int WrapIndex(int value, int wrapLimit)
        {
            int remainder = value % wrapLimit;
            return remainder == 0 ? wrapLimit : remainder;
        }

        private void InstantiateAndBegin(Level targetLevel)
        {
            ActiveLevelInstance = Instantiate(targetLevel);
            ActiveLevelInstance.Load();
            Time.timeScale = 1f;
            if (SerapKeremGameKit._InputSystem.InputHandler.Instance != null)
            {
                SerapKeremGameKit._InputSystem.InputHandler.Instance.UnlockInput();
            }
            StateManager.Instance.SetLoading();
            // UI: update level text on load
            //UIManager.Instance?.RefreshLevelNumber();
            StartLevel();
        }

        #endregion

        #region Level Control Methods

        public void StartLevel()
        {
            ActiveLevelInstance.Play();
            StateManager.Instance.SetOnStart();
            // UI: show in-game UI and refresh text
            //UIManager.Instance?.ShowInGameUI();
            //UIManager.Instance?.RefreshLevelNumber();
        }

        public void RetryLevel()
        {
            TerminateCurrentLevel();
            var retryTarget = _levels[ProcessedLevelIndex - 1];
            InstantiateAndBegin(retryTarget);
        }

        public void RestartLevel()
        {
            StateManager.Instance.SetOnRestart();
            RetryLevel();
        }

        public void CleanCurrentLevel()
        {
            TerminateCurrentLevel();
            // UI: hide gameplay UI if needed
            //UIManager.Instance?.HideInGameUI();
        }

        public void IncreaseLevelNumber()
        {
            TerminateCurrentLevel();
            ActiveLevelNumber++;
        }

        private void TerminateCurrentLevel()
        {
            // notify state if needed
            if (ActiveLevelInstance != null)
                Destroy(ActiveLevelInstance.gameObject);
        }

        #endregion

        #region Game Result Handlers
        [Button("Test LevelWin")]
        public void Win()
        {
            if (!ValidateGameStateForEvents()) return;
            StateManager.Instance.SetOnWin();
            // Example: level-based coin reward
            // Currency.Currency.Add(ActiveLevelNumber * 5);
            // UI: show win panel
            //UIManager.Instance?.ShowWinScreen();
        }

        [Button("Test LevelWin")]
        public void Win(int moveCount)
        {
            if (!ValidateGameStateForEvents()) return;
            StateManager.Instance.SetOnWin();
            // Example: move-based bonus
            // int bonus = Mathf.Max(0, 10 - moveCount);
            // Currency.Currency.Add(bonus);
        }

        [Button("Test LevelLose")]
        public void Lose()
        {
            if (!ValidateGameStateForEvents()) return;
            StateManager.Instance.SetOnLose();
            // UI: show fail panel
            //UIManager.Instance?.ShowFailScreen();
        }

        private bool ValidateGameStateForEvents()
        {
            return StateManager.Instance.CurrentState == GameState.OnStart;
        }

        #endregion

        #region Utility & Validation Methods

        private void PerformInitialValidation()
        {
            if (_levels == null || _levels.Length == 0)
                TraceLogger.LogWarning($"{name}: Levels array is not configured.", this);
        }

        private void ConfigureEnvironment()
        {
#if UNITY_EDITOR
            CleanupExistingLevelsInEditor();
#endif
        }

#if UNITY_EDITOR
        private void CleanupExistingLevelsInEditor()
        {
            var existingLevelInstances = FindObjectsByType<Level>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var levelInstance in existingLevelInstances)
                levelInstance.gameObject.SetActive(false);
        }
#endif

        #endregion

        public Level GetLevelByNumber(int levelNumber)
        {
            int gameplayIndex = levelNumber;

            if (gameplayIndex <= 0 || gameplayIndex > _levels.Length) return null;

            return _levels[gameplayIndex - 1];
        }

        #region Utility & Validation Methods
        public Level GetCurrentLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber);
        }

        public Level GetNextLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 1) ?? GetLevelByNumber(1);
        }

        public Level GetNextestLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 2) ?? GetLevelByNumber(1);
        }

        public Level GetFinalLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 3) ?? GetLevelByNumber(1);
        }

        public Level GetFinalNextLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 4) ?? GetLevelByNumber(1);
        }
        #endregion
    }
}