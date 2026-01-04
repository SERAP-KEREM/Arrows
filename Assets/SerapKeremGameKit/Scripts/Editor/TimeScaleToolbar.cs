#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace SerapKeremGameKit._EditorTools
{
    [InitializeOnLoad]
    public static class TimeScaleToolbar
    {
        private struct Preset { public readonly string Label; public readonly float Value; public Preset(string l, float v){ Label=l; Value=v; } }

        private static readonly Preset[] _presets = new[]
        {
            new Preset("x0.10", 0.1f), new Preset("x0.25", 0.25f), new Preset("x0.50", 0.5f), new Preset("x1.00", 1f),
            new Preset("x1.50", 1.5f), new Preset("x2.00", 2f), new Preset("x5.00", 5f), new Preset("x10.00", 10f),
        };

        private static string[] _items;

        static TimeScaleToolbar()
        {
            BuildItems();
            ToolbarExtender.LeftToolbarGUI.Add(DrawToolbar);
        }

        private static void BuildItems()
        {
            _items = new string[_presets.Length + 1];
            _items[0] = $"Time x{Time.timeScale:0.##}";
            for (int i = 0; i < _presets.Length; i++) _items[i + 1] = _presets[i].Label;
        }

        private static void DrawToolbar()
        {
            GUILayout.Space(6);
            EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_SpeedScale"), GUILayout.Width(22));

            using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
            {
                _items[0] = $"Time x{Time.timeScale:0.##}";
                EditorGUI.BeginChangeCheck();
                int idx = EditorGUILayout.Popup(0, _items, GUILayout.Width(120));
                if (EditorGUI.EndChangeCheck() && idx > 0)
                {
                    float ts = _presets[idx - 1].Value;
                    Time.timeScale = ts;
                    foreach (SceneView sv in SceneView.sceneViews)
                        sv.ShowNotification(new GUIContent($"Time Scale: x{ts:0.##}"));
                }
            }
        }
    }
}
#endif


