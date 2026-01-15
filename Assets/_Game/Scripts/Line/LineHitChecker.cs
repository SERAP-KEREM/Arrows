using System;
using UnityEngine;

namespace _Game.Line
{
    public class LineHitChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRaycastGun2D _lineRaycastGun2D;
    
    private bool _active;
    public Action OnLineHit;

    public void Initialize(LineRaycastGun2D lineRaycastGun2D)
    {
        if (lineRaycastGun2D != null)
        {
            _lineRaycastGun2D = lineRaycastGun2D;
        }
        enabled = false;
        _active = false;
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
}