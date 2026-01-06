using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineClick : MonoBehaviour, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Line clicked!");
        _lineDestroyer.StartCountdown();
        _animation.Play(forwardDirection: true);
        _hitChecker.StartChecking();
    }
}