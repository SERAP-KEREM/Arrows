using UnityEngine;

public class LineRaycastGun2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform head; // usually your line head object

    [Header("Ray Settings")]
    [SerializeField] private float rayLength = 5f;
    [SerializeField] private float offset = 0.5f;
    [SerializeField] private LayerMask layerMask = ~0; // default: everything
    [SerializeField] private bool shootEveryFrame = true;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    public RaycastHit2D LastHit { get; private set; }

    private void Reset()
    {
        if (!head) head = transform;
    }

    private void Update()
    {
        if (!shootEveryFrame) return;
        Shoot();
    }

    public bool Shoot()
    {
        if (!EnsureHead()) return false;

        Vector2 origin = GetOrigin();
        Vector2 direction = head.up;

        LastHit = Physics2D.Raycast(origin, direction, rayLength, layerMask);

        if (LastHit.collider != null)
        {
            Debug.Log($"[LineRaycastGun2D] Hit: {LastHit.collider.name}", LastHit.collider);
            return true;
        }

        return false;
    }

    private bool EnsureHead()
    {
        if (head != null) return true;
        head = transform;
        return head != null;
    }

    private Vector2 GetOrigin()
    {
        return (Vector2)head.position + (Vector2)head.up * offset;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        if (!EnsureHead()) return;

        Vector3 origin = GetOrigin();
        Vector3 direction = head.up;

        float length = rayLength;
        Vector3 end = origin + direction * length;
        Color rayColor = Color.red;

        if (Application.isPlaying && LastHit.collider != null)
        {
            end = LastHit.point;
            rayColor = Color.green;
        }

        Gizmos.color = rayColor;
        Gizmos.DrawLine(origin, end);
        Gizmos.DrawSphere(end, 0.06f);
    }
}