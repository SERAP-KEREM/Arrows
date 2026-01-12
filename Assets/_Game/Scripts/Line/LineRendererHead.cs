using UnityEngine;

namespace _Game.Line
{
    [ExecuteAlways]
    public class LineRendererHead : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float rotationOffset = 0f;

    public void Initialize(LineRenderer lineRenderer)
    {
        this.lineRenderer = lineRenderer;
    }

    private void LateUpdate()
    {
        if (!lineRenderer || lineRenderer.positionCount < 2)
            return;

        // Get last two positions
        int last = lineRenderer.positionCount - 1;
        Vector3 end = lineRenderer.GetPosition(last);
        Vector3 prev = lineRenderer.GetPosition(last - 1);

        // Move head to final point
        transform.localPosition = end;

        // Direction vector
        Vector3 dir = (end - prev).normalized;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        // Rotate head forward
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }
}
}