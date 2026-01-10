using System;
using System.Collections;
using UnityEngine;

public class LineDestroyer : MonoBehaviour
{
    [Header("Destroy Settings")]
    [SerializeField] private float destroyDelay = 5f;
    
    private Coroutine _countdownCoroutine;
    private bool _isCountdownActive;

    public event Action OnCountdownStarted;
    public event Action OnCountdownStopped;
    public event Action OnDestroyed;

    public void StartCountdown()
    {
        if (_isCountdownActive) return;

        StopCountdown();
        
        _isCountdownActive = true;
        _countdownCoroutine = StartCoroutine(CountdownCoroutine());
        OnCountdownStarted?.Invoke();
    }

    public void StopCountdown()
    {
        if (!_isCountdownActive) return;

        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = null;
        }
        
        _isCountdownActive = false;
        OnCountdownStopped?.Invoke();
    }

    private IEnumerator CountdownCoroutine()
    {
        yield return new WaitForSeconds(destroyDelay);
        
        if (_isCountdownActive)
        {
            OnDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        StopCountdown();
    }
}


