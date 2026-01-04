using System;
using UnityEngine;

namespace SerapKeremGameKit._Particles
{
    [Serializable]
    public sealed class ParticleData
    {
        [SerializeField] private string _key;
        [SerializeField] private ParticleSystem _prefab;
        [SerializeField] private float _duration = 1f;

        public string Key { get => _key; set => _key = value; }
        public ParticleSystem Prefab { get => _prefab; set => _prefab = value; }
        public float Duration { get => _duration; set => _duration = value; }
    }
}


