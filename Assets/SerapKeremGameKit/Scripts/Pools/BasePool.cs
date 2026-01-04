using System.Collections.Generic;
using UnityEngine;

namespace SerapKeremGameKit._Pools
{
    public abstract class BasePool<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private int _initialSize = 8;
        [SerializeField] private int _maxSize = 64;
        [SerializeField] private Transform _root;

        private readonly List<T> _available = new List<T>();
        private readonly List<T> _inUse = new List<T>();

        protected virtual void Awake()
        {
            if (_root == null)
            {
                GameObject r = new GameObject(typeof(T).Name + "PoolRoot");
                r.transform.SetParent(transform);
                _root = r.transform;
            }
            for (int i = 0; i < _initialSize; i++)
            {
                _available.Add(Create());
            }
        }

        public T Get()
        {
            T item;
            if (_available.Count > 0)
            {
                int last = _available.Count - 1;
                item = _available[last];
                _available.RemoveAt(last);
            }
            else if (_inUse.Count < _maxSize)
            {
                item = Create();
            }
            else
            {
                if (_inUse.Count == 0) return null;
                T stolen = _inUse[0];
                OnStop(stolen);
                OnRecycle(stolen);
                _inUse.RemoveAt(0);
                item = stolen;
            }

            _inUse.Add(item);
            OnGet(item);
            return item;
        }

        public void Recycle(T item)
        {
            if (item == null) return;
            int index = _inUse.IndexOf(item);
            if (index >= 0)
            {
                _inUse.RemoveAt(index);
                OnRecycle(item);
                _available.Add(item);
            }
        }

        public void StopAndRecycle(T item)
        {
            if (item == null) return;
            OnStop(item);
            Recycle(item);
        }

        protected abstract T Create();
        protected abstract void OnGet(T item);
        protected abstract void OnRecycle(T item);
        protected abstract void OnStop(T item);
    }
}