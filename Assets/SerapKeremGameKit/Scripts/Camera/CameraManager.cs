using SerapKeremGameKit._Singletons;
using SerapKeremGameKit._Logging;
using UnityEngine;

namespace SerapKeremGameKit._Camera
{
    public sealed class CameraManager : MonoSingleton<CameraManager>
    {
        [SerializeField] private Transform _gameCamera;
        [SerializeField] private Transform _followTarget;
        [SerializeField] private Vector3 _followOffset;
		[SerializeField] private float _followLerp = 10f; // kept for backward compat (deprecated)
		[SerializeField] private bool _snapOnStart = true;
        
        [Header("Camera Fitting Settings")]
        [SerializeField] private float _padding = 2f;
        [SerializeField] private float _minOrthographicSize = 5f;
        [SerializeField] private float _maxOrthographicSize = 50f;

        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        public void SnapFollow()
        {
            if (_gameCamera == null || _followTarget == null) return;
            _gameCamera.position = _followTarget.position + _followOffset;
        }

		protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            if (_gameCamera == null) { TraceLogger.LogError("Camera is null!", this); return; }

            _initialPosition = _gameCamera.position;
            _initialRotation = _gameCamera.rotation;

			// One-shot follow at startup if configured
			if (_snapOnStart && _followTarget != null)
			{
				SnapFollow();
			}
        }


        public void InitializeCameraPosition(Transform point)
        {
            if (_gameCamera == null || point == null) return;
            _gameCamera.position = point.position;
            _gameCamera.rotation = point.rotation;
        }

        public void SetFollowTarget(Transform target, Vector3 offset)
        {
            _followTarget = target;
            _followOffset = offset;
        }

		[System.Obsolete("Continuous follow is deprecated. Use SnapFollow/InitializeCameraPosition for one-shot.")]
		public void StepFollow(float deltaTime)
        {
            if (_gameCamera == null || _followTarget == null) return;
            Vector3 targetPos = _followTarget.position + _followOffset;
            _gameCamera.position = Vector3.Lerp(_gameCamera.position, targetPos, deltaTime * _followLerp);
        }

        public void ResetCamera()
        {
            if (_gameCamera == null) return;
            _gameCamera.position = _initialPosition;
            _gameCamera.rotation = _initialRotation;
        }

        public void FitCameraToLines(Transform linesParent)
        {
            if (_gameCamera == null)
            {
                TraceLogger.LogError("Game camera is null. Cannot fit camera to lines.", this);
                return;
            }

            Camera cam = _gameCamera.GetComponent<Camera>();
            if (cam == null)
            {
                TraceLogger.LogError("Camera component not found on game camera transform.", this);
                return;
            }

            if (linesParent == null)
            {
                TraceLogger.LogWarning("Lines parent is null. Cannot fit camera to lines.", this);
                return;
            }

            Bounds bounds = CalculateLinesBounds(linesParent);
            
            if (bounds.size.magnitude < 0.001f)
            {
                TraceLogger.LogWarning("Lines bounds are invalid. Cannot fit camera.", this);
                return;
            }

            Vector3 center = bounds.center;
            center.z = _gameCamera.position.z;

            _gameCamera.position = center;

            if (cam.orthographic)
            {
                float orthographicSize = CalculateOrthographicSize(bounds, cam.aspect);
                orthographicSize = Mathf.Clamp(orthographicSize, _minOrthographicSize, _maxOrthographicSize);
                cam.orthographicSize = orthographicSize;
            }
        }

        private Bounds CalculateLinesBounds(Transform linesParent)
        {
            LineRenderer[] lineRenderers = linesParent.GetComponentsInChildren<LineRenderer>(true);
            
            if (lineRenderers == null || lineRenderers.Length == 0)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            bool hasValidPoint = false;
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (LineRenderer lineRenderer in lineRenderers)
            {
                if (lineRenderer == null || lineRenderer.positionCount < 2)
                    continue;

                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    Vector3 localPos = lineRenderer.GetPosition(i);
                    Vector3 worldPos;

                    if (lineRenderer.useWorldSpace)
                    {
                        worldPos = localPos;
                    }
                    else
                    {
                        worldPos = lineRenderer.transform.TransformPoint(localPos);
                    }

                    if (!hasValidPoint)
                    {
                        minX = worldPos.x;
                        maxX = worldPos.x;
                        minY = worldPos.y;
                        maxY = worldPos.y;
                        hasValidPoint = true;
                    }
                    else
                    {
                        minX = Mathf.Min(minX, worldPos.x);
                        maxX = Mathf.Max(maxX, worldPos.x);
                        minY = Mathf.Min(minY, worldPos.y);
                        maxY = Mathf.Max(maxY, worldPos.y);
                    }
                }
            }

            if (!hasValidPoint)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
            
            return new Bounds(center, size);
        }

        private float CalculateOrthographicSize(Bounds bounds, float aspectRatio)
        {
            float width = bounds.size.x + (_padding * 2f);
            float height = bounds.size.y + (_padding * 2f);

            float sizeByWidth = width / (2f * aspectRatio);
            float sizeByHeight = height / 2f;

            return Mathf.Max(sizeByWidth, sizeByHeight);
        }
    }
}


