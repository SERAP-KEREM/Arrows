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
        [SerializeField] private bool _use2DColliders = true;

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
            if (IsPointerOverUI()) return;

            if (_use2DColliders)
            {
                Handle2DSelection(screenPos);
            }
            else
            {
                Handle3DSelection(screenPos);
            }
        }

        private void Handle2DSelection(Vector3 screenPos)
        {
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            mouseWorldPos.z = 0f;

            Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos, _selectableLayerMash);
            
            if (hitCollider != null)
            {
                GameObject selectedObject = hitCollider.gameObject;
                
                ISelectable selectable = selectedObject.GetComponent<ISelectable>();
                if (selectable == null)
                {
                    selectable = selectedObject.GetComponentInParent<ISelectable>();
                }
                
                if (selectable != null)
                {
                    ProcessSelection(selectable, mouseWorldPos);
                    DrawDebugRay2D(mouseWorldPos, Color.green);
                }
                else
                {
                    DrawDebugRay2D(mouseWorldPos, Color.yellow);
                }
            }
            else
            {
                DrawDebugRay2D(mouseWorldPos, Color.red);
            }
        }

        private void Handle3DSelection(Vector3 screenPos)
        {
            Ray ray = _mainCamera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out var hit, _raycastDistance, _selectableLayerMash))
            {
                if (hit.collider)
                {
                    GameObject selectedObject = hit.collider.gameObject;
                    
                    ISelectable selectable = selectedObject.GetComponent<ISelectable>();
                    if (selectable == null)
                    {
                        selectable = selectedObject.GetComponentInParent<ISelectable>();
                    }
                    
                    if (selectable != null)
                    {
                        ProcessSelection(selectable, hit.point);
                        DrawDebugRay(ray, hit.distance, Color.green);
                    }
                    else
                    {
                        DrawDebugRay(ray, hit.distance, Color.yellow);
                    }
                }
            }
            else
            {
                DrawDebugRay(ray, _raycastDistance, Color.red);
            }
        }

        private void ProcessSelection(ISelectable selectable, Vector3 worldPosition)
        {
            if (selectable != null)
            {
                selectable.OnSelected(worldPosition);
            }
        }

        private void DrawDebugRay2D(Vector3 worldPos, Color color)
        {
            if (_enableDebugRay)
            {
                Debug.DrawRay(worldPos, Vector3.up * 0.1f, color, 1f);
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