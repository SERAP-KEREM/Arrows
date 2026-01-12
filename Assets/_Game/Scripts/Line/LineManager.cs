using System.Collections.Generic;
using UnityEngine;
using SerapKeremGameKit._Logging;
using TriInspector;

namespace _Game.Line
{
    public class LineManager : MonoBehaviour
{
    [Header("Active Lines")]
    [SerializeField, ReadOnly] 
    private List<Line> _activeLines = new();

    [ShowInInspector, ReadOnly]
    public int ActiveLineCount => _activeLines.Count;

    public IReadOnlyList<Line> ActiveLines => _activeLines;

    public void InitializeLines(Transform levelRoot)
    {
        ClearLines();
        
        if (levelRoot == null)
        {
            TraceLogger.LogWarning("Level root is null. Cannot initialize lines.", this);
            return;
        }

        Line[] lines = levelRoot.GetComponentsInChildren<Line>(true);
        
        if (lines == null || lines.Length == 0)
        {
            LineRenderer[] lineRenderers = levelRoot.GetComponentsInChildren<LineRenderer>(true);
            
            if (lineRenderers != null && lineRenderers.Length > 0)
            {
                List<Line> foundLines = new List<Line>();
                
                foreach (LineRenderer lr in lineRenderers)
                {
                    if (lr == null) continue;
                    
                    Line lineComponent = lr.GetComponent<Line>();
                    if (lineComponent == null)
                    {
                        lineComponent = lr.gameObject.AddComponent<Line>();
                    }
                    
                    if (lineComponent != null)
                    {
                        foundLines.Add(lineComponent);
                    }
                }
                
                lines = foundLines.ToArray();
            }
            else
            {
                return;
            }
        }

        foreach (Line line in lines)
        {
            if (line != null)
            {
                RegisterLine(line);
                line.Initialize();
            }
        }
    }

    public void RegisterLine(Line line)
    {
        if (line == null) return;

        if (!_activeLines.Contains(line))
        {
            _activeLines.Add(line);
        }
    }

    public void UnregisterLine(Line line)
    {
        if (line == null) return;

        _activeLines.Remove(line);
    }

    public void ClearLines()
    {
        foreach (Line line in _activeLines)
        {
            if (line != null)
            {
                line.Cleanup();
            }
        }

        _activeLines.Clear();
    }

    public void DestroyAllLines()
    {
        for (int i = _activeLines.Count - 1; i >= 0; i--)
        {
            if (_activeLines[i] != null)
            {
                Destroy(_activeLines[i].gameObject);
            }
        }

        _activeLines.Clear();
    }

    public Line GetLineByIndex(int index)
    {
        if (index < 0 || index >= _activeLines.Count)
            return null;

        return _activeLines[index];
    }
}
}
