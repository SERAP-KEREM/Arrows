using UnityEngine;

namespace _Game.Line
{
    [ExecuteAlways]
    public class LineRendererSnapFixer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lr;

    [Header("Snap Settings")]
    public float snapSize = 1f;
    public bool snapInEditor = true;
    public bool snapAtRuntime = false;

    public void Initialize(LineRenderer lineRenderer)
    {
        lr = lineRenderer;
    }

    private void LateUpdate()
    {
        if (!lr) return;

        if (!Application.isPlaying && !snapInEditor)
            return;

        if (Application.isPlaying && !snapAtRuntime)
            return;

        SnapPositions();
    }

    private void SnapPositions()
    {
        int count = lr.positionCount;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 p = lr.GetPosition(i);
            p.z = 0;
            p = Snap(p);
            lr.SetPosition(i, p);
        }
    }

    [ContextMenu("Clear LineRenderer Positions")]
    private void Clear()
    {
        lr.positionCount = 0;
    }

    private Vector3 Snap(Vector3 pos)
    {
        float s = snapSize;
        pos.x = Mathf.Round(pos.x / s) * s;
        pos.y = Mathf.Round(pos.y / s) * s;
        pos.z = Mathf.Round(pos.z / s) * s;
        return pos;
    }
}
}