using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineClick : MonoBehaviour, IPointerClickHandler
{
    private LineAnimation _animation;
    private LineHitChecker _hitChecker;
    private LineDestroyer _lineDestroyer;
    private Camera _mainCamera;

    private void Start()
    {
        _animation = GetComponent<LineAnimation>();
        _hitChecker = GetComponent<LineHitChecker>();
        _lineDestroyer = GetComponent<LineDestroyer>();
        _hitChecker.OnLineHit += HandleLineHit;
        
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            _mainCamera = FindObjectOfType<Camera>();
        }
        
        EnsurePhysics2DRaycaster();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckManualClick();
        }
    }

    private void CheckManualClick()
    {
        if (_mainCamera == null) return;
        
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        
        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        
        foreach (var col in allColliders)
        {
            if (col == null || !col.enabled) continue;
            
            if (col.OverlapPoint(mouseWorldPos))
            {
                PointerEventData fakeEventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition,
                    pointerCurrentRaycast = new UnityEngine.EventSystems.RaycastResult
                    {
                        worldPosition = mouseWorldPos,
                        gameObject = col.gameObject
                    }
                };
                
                OnPointerClick(fakeEventData);
                return;
            }
        }
    }

    private void EnsurePhysics2DRaycaster()
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null) return;

        if (eventSystem.GetComponent<Physics2DRaycaster>() == null)
        {
            eventSystem.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }

    private void HandleLineHit()
    {
        _animation.Play(forwardDirection: false);
        _lineDestroyer.StopCountdown();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_animation == null || _lineDestroyer == null || _hitChecker == null)
            return;
        
        _lineDestroyer.StartCountdown();
        _animation.Play(forwardDirection: true);
        _hitChecker.StartChecking();
    }
}