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
    }

    private void Update()
    {
        if (!_active) return;

        if (!_lineRaycastGun2D.Shoot())
            return;

        Debug.Log("Line hit detected!");
        _active = false;
        OnLineHit?.Invoke();
    }
}