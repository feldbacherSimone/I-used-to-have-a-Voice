using UnityEngine;
using UnityEngine.InputSystem;

namespace _IUTHAV.Scripts.Core.Input {
    public static class InputController {

        private static InputAction _AInteract;
        private static InputAction _ADrag;
        
        public delegate void InteractionHandler(InputAction.CallbackContext context);

        public static InteractionHandler OnCustomClick;

        public static InteractionHandler OnBeginDrag;

        public static InteractionHandler OnEndDrag;

        public static bool IsConfigured;

        public static bool IsHoldingElement;

        public static void Configure() {

            if (IsConfigured) return;
            
            InputActions actions = new InputActions();

            _AInteract = actions.BaseActionMap.Interact;
            _ADrag = actions.BaseActionMap.Drag;
            
            actions.Enable();

            _AInteract.performed += OnInteractDelegate;
            _ADrag.started += OnBeginDragDelegate;
            _ADrag.canceled += OnEndDragDelegate;

            IsConfigured = true;

        }

        public static void Dispose() {

            _AInteract.performed -= OnInteractDelegate;

            _ADrag.started -= OnBeginDragDelegate;

            _ADrag.canceled -= OnEndDragDelegate;
        }

        private static void OnInteractDelegate(InputAction.CallbackContext context) {
            
            OnCustomClick?.Invoke(context);
            
        }

        private static void OnBeginDragDelegate(InputAction.CallbackContext context) {
            
            OnBeginDrag?.Invoke(context);
            
        }
        
        private static void OnEndDragDelegate(InputAction.CallbackContext context) {
            
            OnEndDrag?.Invoke(context);
            
        }

    }
}
