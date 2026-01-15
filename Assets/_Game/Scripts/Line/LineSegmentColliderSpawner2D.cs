using System.Collections.Generic;
using UnityEngine;

namespace _Game.Line
{
public class LineSegmentColliderSpawner2D : MonoBehaviour
{
    [Header("References")]
    private LineRenderer lineRenderer;
    [SerializeField] private GameObject segmentPrefab;

    [Header("Collider Settings")]
    [SerializeField] private float thickness = 0.2f;
    [SerializeField] private float extraLenght = 0.2f;
    [SerializeField] private bool autoUpdateInPlayMode = true;

    private readonly List<GameObject> _spawnedSegments = new();

    public void Initialize(LineRenderer lineRenderer)
    {
        this.lineRenderer = lineRenderer;

        if (autoUpdateInPlayMode && Application.isPlaying)
        {
            RebuildSegments();
        }
    }

    [ContextMenu("Rebuild Segment Colliders")]
    private void RebuildSegmentsContextMenu()
    {
        RebuildSegments();
    }

    private void RebuildSegments()
    {
        if (!lineRenderer || !segmentPrefab)
            return;

        int count = lineRenderer.positionCount;
        if (count < 2)
        {
            ClearSegments();
            return;
        }

        ClearSegments();

        bool useWorld = lineRenderer.useWorldSpace;

        for (int i = 0; i < count - 1; i++)
        {
            Vector3 a = lineRenderer.GetPosition(i);
            Vector3 b = lineRenderer.GetPosition(i + 1);

            if (!useWorld)
            {
                a = lineRenderer.transform.TransformPoint(a);
                b = lineRenderer.transform.TransformPoint(b);
            }

            Vector3 dir = b - a;
            float length = dir.magnitude;

            GameObject instance = Instantiate(segmentPrefab, transform);
            instance.transform.position = (a + b) / 2f;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            instance.transform.rotation = Quaternion.Euler(0, 0, angle);

            BoxCollider2D box = instance.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                Vector2 size = box.size;
                size.x = length + extraLenght;
                size.y = thickness;
                box.size = size;
                box.offset = Vector2.zero;
            }

            _spawnedSegments.Add(instance);
        }
    }

    public void UpdateSegments()
    {
        if (!lineRenderer || !segmentPrefab)
            return;

        int count = lineRenderer.positionCount;
        if (count < 2)
        {
            ClearSegments();
            return;
        }

        int segmentCount = count - 1;
        bool useWorld = lineRenderer.useWorldSpace;

        if (_spawnedSegments.Count != segmentCount)
        {
            RebuildSegments();
            return;
        }

        for (int i = 0; i < segmentCount && i < _spawnedSegments.Count; i++)
        {
            GameObject segment = _spawnedSegments[i];
            if (segment == null) continue;

            Vector3 a = lineRenderer.GetPosition(i);
            Vector3 b = lineRenderer.GetPosition(i + 1);

            if (!useWorld)
            {
                a = lineRenderer.transform.TransformPoint(a);
                b = lineRenderer.transform.TransformPoint(b);
            }

            Vector3 dir = b - a;
            float length = dir.magnitude;

            segment.transform.position = (a + b) / 2f;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            segment.transform.rotation = Quaternion.Euler(0, 0, angle);

            BoxCollider2D box = segment.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                Vector2 size = box.size;
                size.x = length + extraLenght;
                size.y = thickness;
                box.size = size;
            }
        }
    }

    public void ClearSegments()
    {
        foreach (var go in _spawnedSegments)
        {
            if (!go) continue;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(go);
            else
                Destroy(go);
#else
            Destroy(go);
#endif
        }
        _spawnedSegments.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            
            if (child.GetComponent<LineRendererHead>() != null)
            {
                continue;
            }
            
            if (child.name.Contains("Clone") || child.name.Contains("Segment") || child.GetComponent<Collider2D>() != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(child.gameObject);
                else
                    Destroy(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
        }
    }
    }
}