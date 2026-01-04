using System;
using UnityEngine;

namespace SerapKeremGameKit._Audio
{
    [Serializable]
    public sealed class AudioData
    {
        [SerializeField] private string _key;
        [SerializeField] private AudioClip _clip;
        [SerializeField, Range(0f, 1f)] private float _volume = 1f;
        [SerializeField, Range(0.1f, 3f)] private float _pitch = 1f;
        [SerializeField] private bool _loop = false;
        [SerializeField] private bool _spatial = false;
        [SerializeField] private float _minDistance = 1f;
        [SerializeField] private float _maxDistance = 20f;

        public string Key { get => _key; set => _key = value; }
        public AudioClip Clip { get => _clip; set => _clip = value; }
        public float Volume { get => _volume; set => _volume = value; }
        public float Pitch { get => _pitch; set => _pitch = value; }
        public bool Loop { get => _loop; set => _loop = value; }
        public bool Spatial { get => _spatial; set => _spatial = value; }
        public float MinDistance { get => _minDistance; set => _minDistance = value; }
        public float MaxDistance { get => _maxDistance; set => _maxDistance = value; }
    }
}


