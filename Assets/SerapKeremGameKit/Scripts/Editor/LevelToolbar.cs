#if UNITY_EDITOR
using SerapKeremGameKit._Managers;
using SerapKeremGameKit._LevelSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

namespace SerapKeremGameKit._EditorTools
{
    [InitializeOnLoad]
    public static class LevelToolbar
    {
        private const string PrefSelectedLevel = "serapkerem.toolbar.playLevel";
        private static string[] _levelDisplayOptions;
        private static int _cachedTotalCount;

        static LevelToolbar()
        {
            ToolbarExtender.RightToolbarGUI.Add(DrawToolbar);
        }

        private static void DrawToolbar()
        {
			var lm = Object.FindFirstObjectByType<LevelManager>();
            if (lm == null) return;

            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                EnsureCache(lm);

                int selected = 0;
                EditorGUI.BeginChangeCheck();
                selected = EditorGUILayout.Popup(0, _levelDisplayOptions, GUILayout.Width(140));
                if (EditorGUI.EndChangeCheck() && selected > 0)
                {
                    int targetLevelNumber = selected;
                    lm.ActiveLevelNumber = targetLevelNumber;
                    EditorPrefs.SetInt(PrefSelectedLevel, targetLevelNumber);

                    ShowGameViewNotification($"Play Level {targetLevelNumber}");

                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                    EditorApplication.isPlaying = true;
                    EditorApplication.delayCall += () =>
                    {
                        var liveLm = LevelManager.Instance;
                        if (liveLm != null)
                        {
                            liveLm.ActiveLevelNumber = targetLevelNumber;
                            liveLm.StartCurrentLevelInstance();
                        }
                    };
                }

                GUILayout.Space(6);
                if (GUILayout.Button(new GUIContent("ClearPrefs", "Delete all PlayerPrefs"), GUILayout.Width(90)))
                {
                    if (EditorUtility.DisplayDialog("Clear PlayerPrefs", "Delete ALL PlayerPrefs? This cannot be undone.", "Yes", "No"))
                    {
                        PlayerPrefs.DeleteAll();
                        PlayerPrefs.Save();
                        ShowGameViewNotification("PlayerPrefs cleared");
                    }
                }
            }
        }

        private static void EnsureCache(LevelManager lm)
        {
            int gameplayCount = lm.GameplayLevels != null ? lm.GameplayLevels.Length : 0;
            int total = gameplayCount;
            if (_levelDisplayOptions != null && _cachedTotalCount == total) return;

            _cachedTotalCount = total;
            _levelDisplayOptions = new string[total + 1];
            _levelDisplayOptions[0] = "Play From Level";

            int idx = 1;
            for (int i = 1; i <= gameplayCount; i++, idx++)
                _levelDisplayOptions[idx] = $"{i} - Level";
        }

        private static void ShowGameViewNotification(string message)
        {
            var gvType = typeof(SceneView).Assembly.GetType("UnityEditor.GameView");
            var gv = EditorWindow.GetWindow(gvType);
            gv.ShowNotification(new GUIContent(message));
        }
    }
}
#endif


