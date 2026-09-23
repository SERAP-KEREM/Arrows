using SerapKeremGameKit._Singletons;
using UnityEngine;
using SerapKeremGameKit._InputSystem.Data;

namespace SerapKeremGameKit._InputSystem
{
    [DefaultExecutionOrder(-50)]
    public class InputHandler : MonoSingleton<InputHandler>
    {
        [Header("Input Settings")]
        [SerializeField, Tooltip("Scriptable object for managing player input.")]
        private PlayerInputSO _playerInput;

        private bool _isInputLocked = false; // Indicates whether input is currently locked
        private int _activeFingerId = -1;
        private bool _rejectGesture;
        private bool _mouseGestureActive;

        public bool IsInputLocked { get => _isInputLocked; }

        protected override void Awake()
        {
            base.Awake();

            //if (LoadingPanelController.Instance)
            //{
            //    LockInput();
            //    LoadingPanelController.Instance.OnLoadingFinished += UnlockInput;
            //}
        }

        private void Update()
        {
            if (_playerInput == null) return;
            _playerInput.ResetFrame();

            if (_isInputLocked)
            {
                CancelGesture();
                return;
            }

            if (Input.touchCount > 1)
            {
                CancelGesture();
                _rejectGesture = true;
                return;
            }

            if (_rejectGesture)
            {
                if (Input.touchCount == 0 && !Input.GetMouseButton(0))
                    _rejectGesture = false;
                return;
            }

            if (Input.touchCount == 1)
            {
                ProcessTouchInput(Input.GetTouch(0));
                return;
            }

            if (_activeFingerId != -1)
            {
                CancelGesture();
                return;
            }

            ProcessMouseInput();
        }

        private void ProcessTouchInput(Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _activeFingerId = touch.fingerId;
                _playerInput.SetMouseDown(touch.position);
            }
            else if (touch.fingerId == _activeFingerId)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    _playerInput.SetMouseUp(touch.position);
                    _activeFingerId = -1;
                }
                else if (touch.phase == TouchPhase.Canceled)
                    CancelGesture();
                else
                    _playerInput.SetMouseHeld(touch.position);
            }
        }

        private void ProcessMouseInput()
        {
            Vector3 mousePosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                _mouseGestureActive = true;
                HandleMouseDown(mousePosition);
            }
            else if (_mouseGestureActive && Input.GetMouseButton(0))
            {
                HandleMouseHeld(mousePosition);
            }
            else if (_mouseGestureActive && Input.GetMouseButtonUp(0))
            {
                _mouseGestureActive = false;
                HandleMouseUp(mousePosition);
            }
        }

        private void CancelGesture()
        {
            _activeFingerId = -1;
            _mouseGestureActive = false;
            if (_playerInput != null) _playerInput.Cancel();
        }

        private void OnDisable() => CancelGesture();

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                CancelGesture();
                _rejectGesture = true;
            }
        }

        private void HandleMouseDown(Vector3 position)
        {
            _playerInput.SetMouseDown(position);
        }

        private void HandleMouseHeld(Vector3 position)
        {
            _playerInput.SetMouseHeld(position);
        }

        private void HandleMouseUp(Vector3 position)
        {
            _playerInput.SetMouseUp(position);
        }

        public void UnlockInput()
        {
            _isInputLocked = false;
        }

        public void LockInput()
        {
            _isInputLocked = true;
            CancelGesture();
        }
    }
}
