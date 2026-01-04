using System.Collections.Generic;
using SerapKeremGameKit._Singletons;
using UnityEngine;

namespace SerapKeremGameKit._UI
{
    public sealed class CoinPool : MonoSingleton<CoinPool>
    {
        [SerializeField] private RectTransform _poolRoot;
        [SerializeField] private RectTransform _coinPrefab;
        [SerializeField] private int _prewarmCount = 16;

        private readonly Queue<RectTransform> _pool = new Queue<RectTransform>();

        protected override void Awake()
        {
            base.Awake();
            if (_poolRoot == null) _poolRoot = (RectTransform)transform;
            Prewarm();
        }

        private void Prewarm()
        {
            if (_coinPrefab == null || _prewarmCount <= 0) return;
            for (int i = 0; i < _prewarmCount; i++)
            {
                var c = Instantiate(_coinPrefab, _poolRoot);
                c.gameObject.SetActive(false);
                _pool.Enqueue(c);
            }
        }

        public RectTransform Spawn()
        {
            RectTransform rt = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(_coinPrefab, _poolRoot);
            rt.gameObject.SetActive(true);
            return rt;
        }

        public void Despawn(RectTransform rt)
        {
            if (rt == null) return;
            rt.gameObject.SetActive(false);
            rt.SetParent(_poolRoot, false);
            _pool.Enqueue(rt);
        }
    }
}


