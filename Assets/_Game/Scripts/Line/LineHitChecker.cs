using System;
using UnityEngine;

public class LineHitChecker : MonoBehaviour
{
    private LineRaycastGun2D _lineRaycastGun2D;
    private bool _active;
    public Action OnLineHit;

    private void Start()
    {
        _lineRaycastGun2D = GetComponent<LineRaycastGun2D>();
    }

    public void StartChecking()
    {
        _active = true;
        enabled = true;
    }

    public void StopChecking()
    {
        _active = false;
        enabled = false;
    }

    private void Update()
    {
        if (!_active) return;

        if (!_lineRaycastGun2D.Shoot())
            return;

        _active = false;
        OnLineHit?.Invoke();
    }
}