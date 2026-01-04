using UnityEngine;

namespace SerapKeremGameKit._InputSystem.Data
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "Data/PlayerInput")]
    public class PlayerInputSO : ScriptableObject
    {
        public Vector3 MousePosition { get; private set; }

        public bool DownThisFrame { get; private set; }
        public bool Held { get; private set; }
        public bool UpThisFrame { get; private set; }

        public void ResetFrame()
        {
            DownThisFrame = false;
            UpThisFrame = false;
        }

        public void SetMouseDown(Vector3 position)
        {
            MousePosition = position;
            Held = true;
            DownThisFrame = true;
        }

        public void SetMouseHeld(Vector3 position)
        {
            MousePosition = position;
        }

        public void SetMouseUp(Vector3 position)
        {
            MousePosition = position;
            Held = false;
            UpThisFrame = true;
        }
    }
}