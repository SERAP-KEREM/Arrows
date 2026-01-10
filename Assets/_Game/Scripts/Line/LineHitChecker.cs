using System;
using UnityEngine;

public class LineHitChecker : MonoBehaviour
{
    private LineRaycastGun2D _lineRaycastGun2D;
    private bool _active;
    public Action OnLineHit;

    public void Initialize(LineRaycastGun2D lineRaycastGun2D)
    {
        _lineRaycastGun2D = lineRaycastGun2D;
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
        if (!_active || _lineRaycastGun2D == null) return;

        if (!_lineRaycastGun2D.Shoot())
            return;

        _active = false;
        OnLineHit?.Invoke();
    }
}