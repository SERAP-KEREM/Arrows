using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SerapKeremGameKit._Logging;
using TriInspector;

namespace _Game.Line
{
    public class LineManager : MonoBehaviour
{
    [Header("Active Lines")]
    [SerializeField, ReadOnly] 
    private List<Line> _activeLines = new();

    [Header("Pool Reference")]
    [SerializeField] private Vector3ArrayPool _vector3ArrayPool;

    [ShowInInspector, ReadOnly]
    public int ActiveLineCount => _activeLines.Count;

    public IReadOnlyList<Line> ActiveLines => _activeLines;
    public Vector3ArrayPool Vector3ArrayPool => _vector3ArrayPool;

    private readonly List<RaycastHit2D> _hintHits = new();

    public bool HasMovingLines
    {
        get
        {
            foreach (Line line in _activeLines)
            {
                if (line != null && line.Animation != null && line.Animation.IsPlaying) return true;
            }
            return false;
        }
    }

    public bool TryGetHint(out Line hint)
    {
        hint = null;
        if (HasMovingLines) return false;

        Physics2D.SyncTransforms();
        PhysicsScene2D physicsScene = gameObject.scene.GetPhysicsScene2D();
        var filter = new ContactFilter2D { useTriggers = true };
        foreach (Line candidate in _activeLines)
        {
            if (candidate == null || !candidate.IsClickable || !candidate.gameObject.activeInHierarchy) continue;
            LineRenderer renderer = candidate.LineRenderer;
            CircleCollider2D head = candidate.HeadCollider;
            if (renderer == null || renderer.positionCount < 2 || head == null) continue;

            int last = renderer.positionCount - 1;
            Vector3 origin = renderer.GetPosition(last);
            Vector3 direction = candidate.Animation.Direction;
            if (!renderer.useWorldSpace)
            {
                origin = renderer.transform.TransformPoint(origin);
                direction = renderer.transform.TransformVector(direction);
            }
            if (((Vector2)direction).sqrMagnitude < 0.0001f) continue;

            Vector3 scale = head.transform.lossyScale;
            float radius = head.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
            origin += head.transform.TransformVector(head.offset);
            _hintHits.Clear();
            physicsScene.CircleCast(origin, radius, ((Vector2)direction).normalized, Mathf.Infinity, filter, _hintHits);

            bool blocked = false;
            foreach (RaycastHit2D hit in _hintHits)
            {
                Line other = hit.collider.GetComponentInParent<Line>();
                if (other != null && other != candidate)
                {
                    blocked = true;
                    break;
                }
            }
            if (blocked) continue;

            hint = candidate;
            return true;
        }
        return false;
    }

    public event Action OnAllLinesRemoved;

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
                line.Initialize(this);
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

        if (!_activeLines.Remove(line)) return;

        if (_activeLines.Count == 0)
        {
            OnAllLinesRemoved?.Invoke();
        }
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

    public Line GetLineByIndex(int index)
    {
        if (index < 0 || index >= _activeLines.Count)
            return null;

        return _activeLines[index];
    }
}
}
