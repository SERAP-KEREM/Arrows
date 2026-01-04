using SerapKeremGameKit._Particles;
using SerapKeremGameKit._Logging;
using TriInspector;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    [SerializeField] private string _particleKey;
    [SerializeField] private ParticleManager _particleManager;
    [SerializeField] private bool _useKeyboard = true;
    [SerializeField] private KeyCode _playKey = KeyCode.P;

    private void Awake()
    {
		if (_particleManager == null)
			_particleManager = FindFirstObjectByType<ParticleManager>(FindObjectsInactive.Include);
    }

    private void Update()
    {
        if (!_useKeyboard) return;
        if (Input.GetKeyDown(_playKey))
        {
            PlayNow();
        }
    }

    [Button("Play Particle Now")]
    private void PlayNow()
    {
        if (_particleManager == null)
        {
            TraceLogger.LogWarning("ParticleManager is missing.", this);
            return;
        }
        if (string.IsNullOrEmpty(_particleKey))
        {
            TraceLogger.LogWarning("Particle key is empty.", this);
            return;
        }
        _particleManager.PlayParticle(_particleKey, transform.position, transform);
    }
}
