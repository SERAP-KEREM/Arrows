using UnityEngine;
using SerapKeremGameKit._Logging;
using SerapKeremGameKit._Managers;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour
{
    private LineAnimation _animation;
    private LineClick _click;
    private LineHitChecker _hitChecker;
    private LineDestroyer _destroyer;
    private LineSegmentColliderSpawner2D _colliderSpawner;
    private LineRenderer _lineRenderer;

    public LineRenderer LineRenderer => _lineRenderer;
    public LineAnimation Animation => _animation;
    public LineClick Click => _click;
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _animation = GetComponent<LineAnimation>();
        _click = GetComponent<LineClick>();
        _hitChecker = GetComponent<LineHitChecker>();
        _destroyer = GetComponent<LineDestroyer>();
        _colliderSpawner = GetComponent<LineSegmentColliderSpawner2D>();

        ValidateComponents();
    }

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

        if (_lineRenderer == null)
        {
            TraceLogger.LogError($"Cannot initialize {name}: LineRenderer is missing.", this);
            return;
        }

        if (_lineRenderer.positionCount < 2)
        {
            TraceLogger.LogWarning($"Cannot initialize {name}: LineRenderer has less than 2 positions ({_lineRenderer.positionCount}).", this);
            return;
        }

        InjectDependencies();

        IsInitialized = true;

        if (LineManager.IsInitialized)
        {
            LineManager.Instance.RegisterLine(this);
        }
    }

    private void InjectDependencies()
    {
        if (_animation != null)
        {
            _animation.Initialize(_lineRenderer);
        }

        if (_hitChecker != null)
        {
            LineRaycastGun2D raycastGun = GetComponent<LineRaycastGun2D>();
            if (raycastGun != null)
            {
                _hitChecker.Initialize(raycastGun);
            }
        }

        if (_click != null)
        {
            _click.Initialize(_animation, _hitChecker, _destroyer);
        }

        if (_colliderSpawner != null)
        {
            _colliderSpawner.Initialize(_lineRenderer);
        }

        LineRendererHead head = GetComponent<LineRendererHead>();
        if (head != null)
        {
            head.Initialize(_lineRenderer);
        }

        LineRendererSnapFixer snapFixer = GetComponent<LineRendererSnapFixer>();
        if (snapFixer != null)
        {
            snapFixer.Initialize(_lineRenderer);
        }
    }

    public void Cleanup()
    {
        if (!IsInitialized) return;

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
        if (LineManager.IsInitialized)
        {
            LineManager.Instance.UnregisterLine(this);
        }
    }
}
