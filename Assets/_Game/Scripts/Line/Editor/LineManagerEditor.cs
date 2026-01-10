#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LineManager))]
public class LineManagerEditor : Editor
{
    private LineManager _lineManager;

    private void OnEnable()
    {
        _lineManager = (LineManager)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime Info", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField($"Active Lines Count: {_lineManager.ActiveLineCount}");
            
            if (_lineManager.ActiveLineCount > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Active Lines:", EditorStyles.boldLabel);
                
                for (int i = 0; i < _lineManager.ActiveLineCount; i++)
                {
                    Line line = _lineManager.GetLineByIndex(i);
                    if (line != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField($"Line {i}", line, typeof(Line), true);
                        EditorGUILayout.LabelField($"Initialized: {line.IsInitialized}", GUILayout.Width(100));
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
    }
}
#endif
