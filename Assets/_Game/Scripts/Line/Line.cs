using UnityEngine;
using SerapKeremGameKit._Logging;
using SerapKeremGameKit._Managers;

namespace _Game.Line
{
    [RequireComponent(typeof(LineRenderer))]
    public class Line : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LineAnimation _animation;
    [SerializeField] private LineClick _click;
    [SerializeField] private LineHitChecker _hitChecker;
    [SerializeField] private LineDestroyer _destroyer;
    [SerializeField] private LineSegmentColliderSpawner2D _colliderSpawner;
    [SerializeField] private LineRendererHead _lineHead;

    public LineRenderer LineRenderer => _lineRenderer;
    public LineAnimation Animation => _animation;
    public LineClick Click => _click;
    public bool IsInitialized { get; private set; }

    private void ValidateComponents()
    {
        if (_lineRenderer == null)
        {
            TraceLogger.LogError($"{name} requires LineRenderer component.", this);
        }
    }

    public void Initialize()
    {
        if (IsInitialized)
        {
            TraceLogger.LogWarning($"{name} is already initialized.", this);
            return;
        }

        ValidateComponents();

        if (_lineRenderer == null)
        {
            TraceLogger.LogError($"Cannot initialize {name}: LineRenderer is missing. Please assign it in the Inspector.", this);
            return;
        }

        if (_lineRenderer.positionCount < 2)
        {
            TraceLogger.LogWarning($"Cannot initialize {name}: LineRenderer has less than 2 positions ({_lineRenderer.positionCount}).", this);
            return;
        }
        
        InjectDependencies();
        SubscribeToEvents();

        IsInitialized = true;

        if (LevelManager.IsInitialized && 
            LevelManager.Instance.ActiveLevelInstance != null && 
            LevelManager.Instance.ActiveLevelInstance.LineManager != null)
        {
            LevelManager.Instance.ActiveLevelInstance.LineManager.RegisterLine(this);
        }
    }

    private void InjectDependencies()
    {
        if (_animation != null)
        {
            _animation.Initialize(_lineRenderer);
        }

        if (_click != null)
        {
            _click.Initialize(_animation, _hitChecker, _destroyer);
        }

        if (_colliderSpawner != null)
        {
            _colliderSpawner.Initialize(_lineRenderer);
        }

        InitializeLineHead();
    }

    private void InitializeLineHead()
    {
        if (_lineHead == null)
        {
            _lineHead = GetComponentInChildren<LineRendererHead>(true);
        }
        
        if (_lineHead == null)
        {
            _lineHead = CreateLineHead();
        }

        if (_lineHead != null && _lineHead.gameObject != null)
        {
            _lineHead.gameObject.SetActive(true);
            _lineHead.Initialize(_lineRenderer, this);
            _lineHead.OnHeadCollision += HandleHeadCollision;
        }
    }

    private LineRendererHead CreateLineHead()
    {
        GameObject headObj = new GameObject("Head");
        headObj.transform.SetParent(transform);
        headObj.transform.localPosition = Vector3.zero;
        headObj.SetActive(true);
        return headObj.AddComponent<LineRendererHead>();
    }

    private void SubscribeToEvents()
    {
        if (_animation != null)
        {
            _animation.OnLinePositionsChanged += HandleLinePositionsChanged;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (_animation != null)
        {
            _animation.OnLinePositionsChanged -= HandleLinePositionsChanged;
        }
    }


    private void HandleLinePositionsChanged()
    {
        if (_colliderSpawner != null)
        {
            _colliderSpawner.UpdateSegments();
        }
    }

    private void HandleHeadCollision(Collider2D other)
    {
        ReverseLine();
    }

    private void ReverseLine()
    {
        if (_animation != null)
        {
            _animation.Stop();
            _animation.Play(forwardDirection: false);
        }

        if (_destroyer != null)
        {
            _destroyer.StopCountdown();
        }

        if (_hitChecker != null)
        {
            _hitChecker.StopChecking();
        }
    }

    public void Cleanup()
    {
        if (!IsInitialized) return;

        UnsubscribeFromEvents();

        if (_destroyer != null)
        {
            _destroyer.StopCountdown();
        }

        if (_hitChecker != null)
        {
            _hitChecker.StopChecking();
        }

        if (_animation != null)
        {
            _animation.Stop();
        }

        IsInitialized = false;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        if (_lineHead != null)
        {
            _lineHead.OnHeadCollision -= HandleHeadCollision;
        }
        
        if (LevelManager.IsInitialized && 
            LevelManager.Instance.ActiveLevelInstance != null && 
            LevelManager.Instance.ActiveLevelInstance.LineManager != null)
        {
            LevelManager.Instance.ActiveLevelInstance.LineManager.UnregisterLine(this);
        }
    }
}
}
