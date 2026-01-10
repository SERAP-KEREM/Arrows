using System.Collections;
using UnityEngine;

public class LineDestroyer : MonoBehaviour
{
    [Header("Destroy Settings")]
    [SerializeField] private float destroyDelay = 5f;
    
    private Coroutine _countdownCoroutine;
    private bool _isCountdownActive;

    public void StartCountdown()
    {
        StopCountdown();
        
        _isCountdownActive = true;
        _countdownCoroutine = StartCoroutine(CountdownCoroutine());
    }

    public void StopCountdown()
    {
        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = null;
        }
        
        _isCountdownActive = false;
    }

    private IEnumerator CountdownCoroutine()
    {
        yield return new WaitForSeconds(destroyDelay);
        
        if (_isCountdownActive)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        StopCountdown();
    }
}


