using System.Collections;
using UnityEngine;

public class LineDestroyer : MonoBehaviour
{
    [Header("Destroy Settings")]
    [SerializeField] private float destroyDelay = 5f; // Time in seconds before destroying the line
    
    private Coroutine _countdownCoroutine;
    private bool _isCountdownActive;

    /// <summary>
    /// Starts the countdown timer. If the countdown completes, the line will be destroyed.
    /// </summary>
    public void StartCountdown()
    {
        StopCountdown(); // Stop any existing countdown first
        
        _isCountdownActive = true;
        _countdownCoroutine = StartCoroutine(CountdownCoroutine());
    }

    /// <summary>
    /// Stops the countdown timer, preventing the line from being destroyed.
    /// </summary>
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
            Debug.Log("Line destroyed due to timeout!");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Clean up coroutine if object is destroyed externally
        StopCountdown();
    }
}


