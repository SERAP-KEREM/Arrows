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
    }
}


