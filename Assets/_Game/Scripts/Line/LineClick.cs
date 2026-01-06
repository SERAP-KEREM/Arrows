using System;
using UnityEngine;
using SerapKeremGameKit._InputSystem;

public class LineClick : MonoBehaviour, ISelectable
{
    private LineAnimation _animation;
    private LineHitChecker _hitChecker;
    private LineDestroyer _lineDestroyer;

    private void Start()
    {
        _animation = GetComponent<LineAnimation>();
        _hitChecker = GetComponent<LineHitChecker>();
        _lineDestroyer = GetComponent<LineDestroyer>();
        _hitChecker.OnLineHit += HandleLineHit;
    }

    private void HandleLineHit()
    {
        _animation.Play(forwardDirection: false);
        _lineDestroyer.StopCountdown();
    }

    public void OnSelected(Vector3 worldPosition)
    {
        if (_animation == null || _lineDestroyer == null || _hitChecker == null)
            return;
        
        _lineDestroyer.StartCountdown();
        _animation.Play(forwardDirection: true);
        _hitChecker.StartChecking();
    }
}