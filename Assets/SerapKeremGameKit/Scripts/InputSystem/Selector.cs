using System;
using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using SerapKeremGameKit._Logging;
using SerapKeremGameKit._InputSystem.Data;

namespace SerapKeremGameKit._InputSystem
{
    public class Selector : MonoBehaviour
    {
        [Title("Selector Settings")]
        [SerializeField] private PlayerInputSO _playerInputSO;

        [Header("Raycasting")]
        [SerializeField] private LayerMask _selectableLayerMash;
        [SerializeField, Range(10f, 1000f)] private float _raycastDistance = 500f;

        [Header("Debug")]
        [SerializeField] private bool _enableDebugRay = true;

        [Header("Camera")]
        [SerializeField, ReadOnly] private Camera _mainCamera;

        //[Header("Audio")]
        // Integrate with your AudioManager if desired later

        private void Awake()
        {
            _mainCamera = Camera.main;
            ValidateReferences();
        }

        private void Update()
        {
            if (_playerInputSO == null) return;
            if (_playerInputSO.DownThisFrame) HandleSelectStart(_playerInputSO.MousePosition);
            else if (_playerInputSO.Held) HandleDrag(_playerInputSO.MousePosition);
            else if (_playerInputSO.UpThisFrame) HandleRelease(_playerInputSO.MousePosition);
        }

        private void ValidateReferences()
        {
            if (_mainCamera == null)
                TraceLogger.LogError("Main Camera not found. Make sure it is tagged 'MainCamera'.");

            if (_playerInputSO == null)
                TraceLogger.LogError("PlayerInputSO reference is missing.");
        }

        private void HandleSelectStart(Vector3 screenPos)
        {
            //if (!IsGamePlayable()) return;
            if (IsPointerOverUI()) return;

            Ray ray = _mainCamera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out var hit, _raycastDistance, _selectableLayerMash))
            {
                if (hit.collider)
                {
                    // Optional: integrate haptics/audio as needed
                    TraceLogger.Log("select");
                }

                DrawDebugRay(ray, hit.distance, Color.green);
            }
            else
            {
                DrawDebugRay(ray, _raycastDistance, Color.red);
            }
        }

        private void HandleDrag(Vector3 screenPos)
        {
            //if (!IsGamePlayable()) return;
            if (IsPointerOverUI()) return;

            Ray ray = _mainCamera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out var hit, _raycastDistance, _selectableLayerMash))
            {
                if (hit.collider)
                {
                 
                }

                DrawDebugRay(ray, hit.distance, Color.green);
            }
            else
            {
                DrawDebugRay(ray, _raycastDistance, Color.red);
            }
        }

        private void HandleRelease(Vector3 screenPos)
        {
           
        }

        //private bool IsGamePlayable() => StateManager.Instance.CurrentState == GameState.OnStart;

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private void DrawDebugRay(Ray ray, float distance, Color color)
        {
            if (_enableDebugRay)
                Debug.DrawRay(ray.origin, ray.direction * distance, color, 1f);
        }
    }
}