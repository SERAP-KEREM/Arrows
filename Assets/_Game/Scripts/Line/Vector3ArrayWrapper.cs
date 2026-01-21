using UnityEngine;

namespace _Game.Line
{
    public class Vector3ArrayWrapper : MonoBehaviour
    {
        private Vector3[] _array;
        private int _length;

        public Vector3[] Array
        {
            get => _array;
            set
            {
                _array = value;
                _length = value != null ? value.Length : 0;
            }
        }

        public int Length => _length;

        public void ResetState()
        {
            if (_array != null)
            {
                for (int i = 0; i < _array.Length; i++)
                {
                    _array[i] = Vector3.zero;
                }
            }
            _length = 0;
        }

        public void SetLength(int length)
        {
            if (_array == null || _array.Length < length)
            {
                _array = new Vector3[length];
            }
            _length = length;
        }
    }
}
