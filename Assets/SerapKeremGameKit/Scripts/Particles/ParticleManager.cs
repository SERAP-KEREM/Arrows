using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerapKeremGameKit._Singletons;
using SerapKeremGameKit._Logging;

namespace SerapKeremGameKit._Particles
{
    public sealed class ParticleManager : MonoSingleton<ParticleManager>
    {
        [SerializeField] private List<ParticleData> _particleList = new List<ParticleData>();
        [SerializeField] private ParticlePool _pool;

        private readonly Dictionary<string, ParticleData> _keyToData = new Dictionary<string, ParticleData>();

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            BuildRegistry();
            EnsurePool();
        }

        public void PlayParticle(string key, Vector3 position, Transform parent = null)
        {
            if (!_keyToData.TryGetValue(key, out ParticleData data) || data == null || data.Prefab == null)
            {
                TraceLogger.LogWarning("Particle key not found: " + key, this);
                return;
            }

            ParticlePlayer player = _pool.Get();
            if (player == null) return;

            Transform attach = parent != null ? parent : transform;
            player.Play(data.Prefab, position, attach, data.Duration);
            StartCoroutine(RecycleWhenFinished(player));
        }

        private IEnumerator RecycleWhenFinished(ParticlePlayer player)
        {
            while (player != null && player.IsAlive())
            {
                yield return null;
            }
            if (player != null)
            {
                _pool.Recycle(player);
            }
        }

        private void BuildRegistry()
        {
            _keyToData.Clear();
            for (int i = 0; i < _particleList.Count; i++)
            {
                ParticleData d = _particleList[i];
                if (!string.IsNullOrEmpty(d.Key) && d.Prefab != null)
                {
                    _keyToData[d.Key] = d;
                }
            }
        }

        private void EnsurePool()
        {
            if (_pool == null)
            {
                _pool = gameObject.GetComponent<ParticlePool>();
                if (_pool == null)
                {
                    _pool = gameObject.AddComponent<ParticlePool>();
                }
            }
        }
    }
}


