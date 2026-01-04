using SerapKeremGameKit._Audio;
using SerapKeremGameKit._Logging;
using TriInspector;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField] private string _audioKey;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private bool _useKeyboard = true;
    [SerializeField] private KeyCode _playKey = KeyCode.A;

    private void Awake()
    {
		if (_audioManager == null)
			_audioManager = FindFirstObjectByType<AudioManager>(FindObjectsInactive.Include);
    }

    private void Update()
    {
        if (!_useKeyboard) return;
        if (Input.GetKeyDown(_playKey))
        {
            PlayNow();
        }
    }

    [Button("Play Audio Now")]
    private void PlayNow()
    {
        if (_audioManager == null)
        {
            TraceLogger.LogWarning("AudioManager is missing.", this);
            return;
        }
        if (string.IsNullOrEmpty(_audioKey))
        {
            TraceLogger.LogWarning("Audio key is empty.", this);
            return;
        }
        _audioManager.Play(_audioKey);
    }
}
