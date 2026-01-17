using UnityEngine;
using SerapKeremGameKit._Logging;
using _Game.UI;

namespace _Game.Line
{
    [RequireComponent(typeof(LineRenderer))]
    public class Line : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LineAnimation _animation;
    [SerializeField] private LineClick _click;
    [SerializeField] private LineDestroyer _destroyer;
    [SerializeField] private LineSegmentColliderSpawner2D _colliderSpawner;
    [SerializeField] private LineRendererHead _lineHead;
    [SerializeField] private SpriteRenderer _lineHeadSpriteRenderer;
    [SerializeField] private LineMaterialHandler _materialHandler;
    [SerializeField] private LineRendererSnapFixer _snapFixer;

    public LineRenderer LineRenderer => _lineRenderer;
    public LineAnimation Animation => _animation;
    public LineClick Click => _click;
    public bool IsInitialized { get; private set; }
    public bool IsClickable => !_hasCollided && (_animation == null || !_animation.IsPlaying || (_animation.IsPlaying && _animation.IsForward));

    private LineManager _lineManager;
    private Vector3ArrayPool _arrayPool;
    private bool _hasCollided = false;
    private bool _hasLostLifeForThisCollision = false;
    private LineHeadCollisionDetector _headCollisionDetector;

    private void ValidateComponents()
    {
        if (_lineRenderer == null)
        {
            TraceLogger.LogError($"{name} requires LineRenderer component.", this);
        }
    }

    public void Initialize(LineManager lineManager)
    {
        if (IsInitialized)
        {
            TraceLogger.LogWarning($"{name} is already initialized.", this);
            return;
        }

        if (lineManager == null)
        {
            TraceLogger.LogError($"Cannot initialize {name}: LineManager is null.", this);
            return;
        }

        _lineManager = lineManager;
        _arrayPool = _lineManager != null ? _lineManager.Vector3ArrayPool : null;

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
        
        InitializeMaterialHandler();
        InitializeSnapFixer();
        InjectDependencies();
        SubscribeToEvents();
        AddHeadToMaterialHandler();

        IsInitialized = true;

        if (_lineManager != null)
        {
            _lineManager.RegisterLine(this);
        }
    }

    private void InitializeMaterialHandler()
    {
        if (_materialHandler != null && _lineRenderer != null)
        {
            _materialHandler.AddRenderer(_lineRenderer);
        }
    }

    private void AddHeadToMaterialHandler()
    {
        if (_materialHandler == null || _lineHeadSpriteRenderer == null) return;

        _materialHandler.AddRenderer(_lineHeadSpriteRenderer);
    }

    private void InitializeSnapFixer()
    {
        if (_snapFixer != null)
        {
            _snapFixer.Initialize();
        }
    }

    private void InjectDependencies()
    {
        if (_animation != null)
        {
            _animation.Initialize(_lineRenderer, _arrayPool);
        }

        if (_click != null)
        {
            _click.Initialize(_animation, _destroyer, this);
        }

        if (_colliderSpawner != null)
        {
            _colliderSpawner.Initialize(_lineRenderer);
        }

        InitializeLineHead();
    }

    private void InitializeLineHead()
    {
        if (_lineHead != null && _lineHead.gameObject != null)
        {
            _lineHead.gameObject.SetActive(true);
            _lineHead.Initialize(_lineRenderer, this);
            _lineHead.OnHeadCollision += HandleHeadCollision;
            _headCollisionDetector = _lineHead.GetComponent<LineHeadCollisionDetector>();
            _hasCollided = false;
            _hasLostLifeForThisCollision = false;
        }
    }

    private void ResetHeadCollision()
    {
        if (_headCollisionDetector != null)
        {
            _headCollisionDetector.ResetCollision();
        }
    }

    private void SubscribeToEvents()
    {
        if (_animation != null)
        {
            _animation.OnLinePositionsChanged += HandleLinePositionsChanged;
            _animation.OnAnimationStarted += HandleAnimationStarted;
            _animation.OnAnimationStopped += HandleAnimationStopped;
            _animation.OnAnimationCompleted += HandleAnimationCompleted;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (_animation != null)
        {
            _animation.OnLinePositionsChanged -= HandleLinePositionsChanged;
            _animation.OnAnimationStarted -= HandleAnimationStarted;
            _animation.OnAnimationStopped -= HandleAnimationStopped;
            _animation.OnAnimationCompleted -= HandleAnimationCompleted;
        }
    }


    private void HandleLinePositionsChanged()
    {
        if (_colliderSpawner != null)
        {
            _colliderSpawner.UpdateSegments();
        }
    }

    private void HandleAnimationStarted(bool forwardDirection)
    {
        if (forwardDirection)
        {
            _hasCollided = false;
            _hasLostLifeForThisCollision = false;
            ResetHeadCollision();
        }
    }

    private void HandleAnimationStopped()
    {
        if (_animation != null && _animation.IsForward)
        {
            _hasCollided = false;
            _hasLostLifeForThisCollision = false;
        }
        
        ResetHeadCollision();
    }

    private void HandleAnimationCompleted()
    {
        if (_animation == null) return;

        if (!_animation.IsForward)
        {
            _hasCollided = false;
            _hasLostLifeForThisCollision = false;
            return;
        }

        if (_hasCollided) return;

        if (_lineManager != null)
        {
            _lineManager.UnregisterLine(this);
        }
    }

    private void HandleHeadCollision(Collider2D other)
    {
        if (_hasCollided) return;
        ReverseLine();
    }

    private void ReverseLine()
    {
        if (_hasCollided) return;
        _hasCollided = true;

        if (_animation != null)
        {
            _animation.Stop();
            _animation.Play(forwardDirection: false);
        }

        if (_destroyer != null)
        {
            _destroyer.StopCountdown();
        }

        if (_materialHandler != null)
        {
            _materialHandler.SetFailureColor();
        }

        if (!_hasLostLifeForThisCollision && LivesManager.IsInitialized)
        {
            _hasLostLifeForThisCollision = true;
            LivesManager.Instance.LoseLife();
        }
    }

    public void Cleanup()
    {
        if (!IsInitialized) return;

        UnsubscribeFromEvents();

        if (_materialHandler != null)
        {
            _materialHandler.ResetToOriginalColors();
        }

        if (_destroyer != null)
        {
            _destroyer.StopCountdown();
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
        
        if (_lineManager != null)
        {
            _lineManager.UnregisterLine(this);
        }
    }
}
}
