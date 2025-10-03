using UnityEngine;
using Koala.Simulation.Input;
using UnityEngine.InputSystem;

namespace Koala.Simulation.FirstPersonController
{
    public class PlayerInputState : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        [Header("Mouse Cursor Settings")]
        public CursorLockMode cursorLocked = CursorLockMode.Locked;
        public bool cursorInputForLook = true;

        private void OnEnable()
        {
            InputManager.Subscribe("Move", OnMove);
            InputManager.Subscribe("Look", OnLook);
            InputManager.Subscribe("Jump", OnJump);
            InputManager.Subscribe("Sprint", OnSprint);
        }

        private void OnDisable()
        {
            InputManager.Unsubscribe("Move", OnMove);
            InputManager.Unsubscribe("Look", OnLook);
            InputManager.Unsubscribe("Jump", OnJump);
            InputManager.Unsubscribe("Sprint", OnSprint);
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            move = ctx.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            look = ctx.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            jump = ctx.ReadValueAsButton();
        }

        private void OnSprint(InputAction.CallbackContext ctx)
        {
            sprint = ctx.ReadValueAsButton();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(CursorLockMode newState)
        {
            Cursor.lockState = newState;
        }
    }
}