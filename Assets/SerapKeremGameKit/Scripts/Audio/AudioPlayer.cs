using UnityEngine;

namespace SerapKeremGameKit._Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip, float volume, float pitch, bool loop, bool spatial, Vector3? position, float minDistance, float maxDistance)
        {
            if (clip == null) return;
            if (position.HasValue)
            {
                transform.position = position.Value;
            }

            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.loop = loop;
            _audioSource.spatialBlend = spatial ? 1f : 0f;
            _audioSource.minDistance = minDistance;
            _audioSource.maxDistance = maxDistance;
            _audioSource.Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public bool IsPlaying()
        {
            return _audioSource != null && _audioSource.isPlaying;
        }

        public void ResetState()
        {
            _audioSource.clip = null;
            _audioSource.volume = 1f;
            _audioSource.pitch = 1f;
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
        }
    }
}


