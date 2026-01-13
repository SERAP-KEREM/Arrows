using System;
using UnityEngine;

namespace _Game.Line
{
    public class LineRendererHead : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float rotationOffset = 0f;
    
    private LineHeadCollisionDetector _collisionDetector;
    private Line _ownLine;

    public event Action<Collider2D> OnHeadCollision;

    public void Initialize(LineRenderer lineRenderer, Line ownLine = null)
    {
        this.lineRenderer = lineRenderer;
        _ownLine = ownLine;
        
        EnsureActive();
        SetupPhysicsComponents();
        SetupCollisionDetector();
    }

    private void EnsureActive()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    private void SetupPhysicsComponents()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.gravityScale = 0f;
        }
        
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = 0.3f;
        }
        circleCollider.isTrigger = true;
    }

    private void SetupCollisionDetector()
    {
        _collisionDetector = GetComponent<LineHeadCollisionDetector>();
        if (_collisionDetector == null)
        {
            _collisionDetector = gameObject.AddComponent<LineHeadCollisionDetector>();
        }
        
        if (_collisionDetector != null && _ownLine != null)
        {
            _collisionDetector.Initialize(_ownLine);
            _collisionDetector.OnHeadCollision += HandleHeadCollision;
        }
    }
    
    private void Awake()
    {
        EnsureActive();
    }

    private void OnEnable()
    {
        EnsureActive();
    }

    private void Start()
    {
        EnsureActive();
    }
    
    private void HandleHeadCollision(Collider2D other)
    {
        OnHeadCollision?.Invoke(other);
    }
    
    private void OnDestroy()
    {
        if (_collisionDetector != null)
        {
            _collisionDetector.OnHeadCollision -= HandleHeadCollision;
        }
    }
    
    private void OnDisable()
    {
    }

    private void LateUpdate()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        
        if (this == null || gameObject == null)
        {
            return;
        }

        if (!lineRenderer)
        {
            return;
        }

        if (lineRenderer.positionCount < 2)
        {
            return;
        }

        int last = lineRenderer.positionCount - 1;
        Vector3 end = lineRenderer.GetPosition(last);
        Vector3 prev = lineRenderer.GetPosition(last - 1);

        Vector3 headWorldPos = end;
        if (!lineRenderer.useWorldSpace)
        {
            headWorldPos = lineRenderer.transform.TransformPoint(end);
        }

        if (transform.parent == lineRenderer.transform)
        {
            transform.localPosition = end;
        }
        else
        {
            transform.position = headWorldPos;
        }

        Vector3 dir = (end - prev).normalized;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }
}
}