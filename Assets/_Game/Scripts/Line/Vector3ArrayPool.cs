using System.Collections.Generic;
using SerapKeremGameKit._Pools;
using UnityEngine;

namespace _Game.Line
{
    public class Vector3ArrayPool : BasePool<Vector3ArrayWrapper>
    {
        [SerializeField] private int _maxArrayLength = 100;

        private readonly Dictionary<Vector3[], Vector3ArrayWrapper> _arrayToWrapper = new Dictionary<Vector3[], Vector3ArrayWrapper>();

        protected override Vector3ArrayWrapper Create()
        {
            GameObject go = new GameObject();
            go.name = nameof(Vector3ArrayWrapper);
            go.SetActive(false);
            go.transform.SetParent(transform, false);
            
            Vector3ArrayWrapper wrapper = go.AddComponent<Vector3ArrayWrapper>();
            wrapper.SetLength(_maxArrayLength);
            wrapper.ResetState();
            
            return wrapper;
        }

        protected override void OnGet(Vector3ArrayWrapper item)
        {
            item.gameObject.SetActive(true);
            if (item.Array != null)
            {
                _arrayToWrapper[item.Array] = item;
            }
        }

        protected override void OnRecycle(Vector3ArrayWrapper item)
        {
            if (item.Array != null)
            {
                _arrayToWrapper.Remove(item.Array);
            }
            item.ResetState();
            item.gameObject.SetActive(false);
        }

        protected override void OnStop(Vector3ArrayWrapper item)
        {
            item.ResetState();
        }

        public Vector3[] GetArray(int length)
        {
            if (length <= 0 || length > _maxArrayLength)
            {
                return new Vector3[length];
            }

            Vector3ArrayWrapper wrapper = Get();
            if (wrapper == null)
            {
                return new Vector3[length];
            }

            if (wrapper.Array == null || wrapper.Array.Length < length)
            {
                wrapper.SetLength(length);
            }

            return wrapper.Array;
        }

        public void RecycleArray(Vector3[] array)
        {
            if (array == null || array.Length > _maxArrayLength)
                return;

            if (_arrayToWrapper.TryGetValue(array, out Vector3ArrayWrapper wrapper))
            {
                Recycle(wrapper);
            }
        }
    }
}
