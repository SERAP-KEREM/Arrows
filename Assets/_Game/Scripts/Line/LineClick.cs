using System;
using UnityEngine;
using SerapKeremGameKit._InputSystem;

namespace _Game.Line
{
    public class LineClick : MonoBehaviour, ISelectable
{
    private LineAnimation _animation;
    private LineHitChecker _hitChecker;
    private LineDestroyer _lineDestroyer;
    private bool _isInitialized;

    public event Action<Vector3> OnLineSelected;

    public void Initialize(LineAnimation animation, LineHitChecker hitChecker, LineDestroyer lineDestroyer)
    {
        _animation = animation;
        _hitChecker = hitChecker;
        _lineDestroyer = lineDestroyer;
        _isInitialized = true;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (_hitChecker != null)
        {
            _hitChecker.OnLineHit += HandleLineHit;
        }

        if (_animation != null)
        {
            _animation.OnAnimationStopped += HandleAnimationStopped;
        }

        if (_lineDestroyer != null)
        {
            _lineDestroyer.OnDestroyed += HandleLineDestroyed;
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        if (_hitChecker != null)
        {
            _hitChecker.OnLineHit -= HandleLineHit;
        }

        if (_animation != null)
        {
            _animation.OnAnimationStopped -= HandleAnimationStopped;
        }

        if (_lineDestroyer != null)
        {
            _lineDestroyer.OnDestroyed -= HandleLineDestroyed;
        }
    }

    private void HandleLineHit()
    {
        if (_animation != null)
        {
            _animation.Play(forwardDirection: false);
        }
        
        if (_lineDestroyer != null)
        {
            _lineDestroyer.StopCountdown();
        }
    }

    private void HandleAnimationStopped()
    {
        if (_hitChecker != null)
        {
            _hitChecker.StopChecking();
        }
    }

    private void HandleLineDestroyed()
    {
        if (_animation != null)
        {
            _animation.Stop();
        }

        if (_hitChecker != null)
        {
            _hitChecker.StopChecking();
        }
    }

    public void OnSelected(Vector3 worldPosition)
    {
        if (!_isInitialized || _animation == null || _lineDestroyer == null || _hitChecker == null)
            return;
        
        OnLineSelected?.Invoke(worldPosition);
        
        _lineDestroyer.StartCountdown();
        _animation.Play(forwardDirection: true);
        _hitChecker.StartChecking();
    }
}
}