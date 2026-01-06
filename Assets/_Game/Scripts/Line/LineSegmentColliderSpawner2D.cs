using System.Collections.Generic;
using UnityEngine;

public class LineSegmentColliderSpawner2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject segmentPrefab; // MUST have BoxCollider2D

    [Header("Collider Settings")]
    [SerializeField] private float thickness = 0.2f; // collider height
    [SerializeField] private float extraLenght = 0.2f; // collider width extension
    [SerializeField] private bool autoUpdateInPlayMode = true;

    private readonly List<GameObject> _spawnedSegments = new();

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
            // Get segment endpoints
            Vector3 a = lineRenderer.GetPosition(i);
            Vector3 b = lineRenderer.GetPosition(i + 1);

            if (!useWorld)
            {
                // Convert from local to world if LineRenderer is local
                a = lineRenderer.transform.TransformPoint(a);
                b = lineRenderer.transform.TransformPoint(b);
            }

            Vector3 dir = b - a;
            float length = dir.magnitude;

            // Instantiate and position segment
            GameObject instance = Instantiate(segmentPrefab, transform);
            instance.transform.position = (a + b) / 2f;

            // Rotate to align with segment
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            instance.transform.rotation = Quaternion.Euler(0, 0, angle);

            BoxCollider2D box = instance.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                // Make collider cover the whole segment
                Vector2 size = box.size;
                size.x = length + extraLenght; // along X (forward)
                size.y = thickness; // height (thickness)
                box.size = size;
                box.offset = Vector2.zero;
            }

            _spawnedSegments.Add(instance);
        }
    }

    private void ClearSegments()
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
    }
}