using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace com.torrenzo.Foundation {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions {
        public event UnityAction<Vector2> Click = delegate { };
        public event UnityAction<bool> Jump = delegate { };

        PlayerInputActions inputActions;

        void OnEnable() {
            if (inputActions == null) {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }
        }

        public void EnablePlayerActions() {
            inputActions.Enable();
        }

        public void OnJump(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnClick(InputAction.CallbackContext context) {
            if (context.phase == InputActionPhase.Performed) {
                Click.Invoke(context.ReadValue<Vector2>());
            }
        }
    }
}

