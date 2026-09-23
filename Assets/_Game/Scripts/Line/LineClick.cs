using System;
using UnityEngine;
using SerapKeremGameKit._InputSystem;
using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Managers;

namespace _Game.Line
{
    public class LineClick : MonoBehaviour, ISelectable
{
    [SerializeField] private Line _ownLine;

    private LineAnimation _animation;
    private LineDestroyer _lineDestroyer;
    private bool _isInitialized;

    public void Initialize(LineAnimation animation, LineDestroyer lineDestroyer, Line ownLine = null)
    {
        _animation = animation;
        _lineDestroyer = lineDestroyer;
        
        if (ownLine != null)
        {
            _ownLine = ownLine;
        }

        _isInitialized = true;
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
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
        if (_animation != null)
        {
            _animation.OnAnimationStopped -= HandleAnimationStopped;
        }

        if (_lineDestroyer != null)
        {
            _lineDestroyer.OnDestroyed -= HandleLineDestroyed;
        }
    }

    private void HandleAnimationStopped()
    {
    }

    private void HandleLineDestroyed()
    {
        if (_animation != null)
        {
            _animation.Stop();
        }
    }

    public void OnSelected(Vector3 worldPosition)
    {
        if (!StateManager.IsInitialized || StateManager.Instance.CurrentState != GameState.OnStart ||
            (InputHandler.IsInitialized && InputHandler.Instance.IsInputLocked))
            return;

        if (!_isInitialized || _animation == null || _lineDestroyer == null)
            return;

        if (_ownLine != null && !_ownLine.IsClickable)
            return;
        
        _animation.Play(forwardDirection: true);
    }
    }
}
