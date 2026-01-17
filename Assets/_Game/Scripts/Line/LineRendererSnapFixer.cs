using UnityEngine;

namespace _Game.Line
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererSnapFixer : MonoBehaviour
    {
        [Header("Snap Settings")]
        [SerializeField] private float _snapSize = 1f;
        [SerializeField] private bool _snapInEditor = true;
        [SerializeField] private bool _snapAtRuntime = false;

        [Header("References")]
        [SerializeField] private LineRenderer _lineRenderer;

        private bool _isInitialized;

        public float SnapSize => _snapSize;
        public bool SnapInEditor => _snapInEditor;
        public bool SnapAtRuntime => _snapAtRuntime;
        public bool IsInitialized => _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;

            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }

            if (_lineRenderer == null)
            {
                enabled = false;
                return;
            }

            _isInitialized = true;
            UpdateEnabledState();
        }

        private void UpdateEnabledState()
        {
            if (!_isInitialized || _lineRenderer == null)
            {
                enabled = false;
                return;
            }

            bool shouldSnap = false;

            if (!Application.isPlaying && _snapInEditor)
            {
                shouldSnap = true;
            }

            if (Application.isPlaying && _snapAtRuntime)
            {
                shouldSnap = true;
            }

            enabled = shouldSnap;
        }

        private void LateUpdate()
        {
            if (!_isInitialized || _lineRenderer == null) return;

            if (!Application.isPlaying && !_snapInEditor)
                return;

            if (Application.isPlaying && !_snapAtRuntime)
                return;

            SnapPositions();
        }

        private void SnapPositions()
        {
            if (_lineRenderer == null) return;

            int count = _lineRenderer.positionCount;
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                Vector3 position = _lineRenderer.GetPosition(i);
                position.z = 0;
                position = SnapPosition(position);
                _lineRenderer.SetPosition(i, position);
            }
        }

        private Vector3 SnapPosition(Vector3 position)
        {
            float snapSize = _snapSize;
            position.x = Mathf.Round(position.x / snapSize) * snapSize;
            position.y = Mathf.Round(position.y / snapSize) * snapSize;
            position.z = Mathf.Round(position.z / snapSize) * snapSize;
            return position;
        }

        [ContextMenu("Clear LineRenderer Positions")]
        private void ClearPositions()
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.positionCount = 0;
            }
        }

        private void OnValidate()
        {
            UpdateEnabledState();
        }

        private void Awake()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }
    }
}
