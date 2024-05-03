using _IUTHAV.Scripts.Core.Input;
using UnityEngine.InputSystem;

namespace _IUTHAV.Scripts.Interaction {
    public class ClickAndDragObject : SelectAndClickObject {



#region Unity Functions

        private void OnEnable() {
            Configure();
        }

        private void OnDisable() {
            Dispose();
        }

#endregion

#region Public Functions

        public override void Interact() {
            base.Interact();
        }

#endregion

#region Private Functions

        protected override void Configure() {
            base.Configure();
            InputController.OnBeginDrag += OnBeginDrag;
            InputController.OnEndDrag += OnEndDrag;
        }

        protected void OnBeginDrag(InputAction.CallbackContext context) {
            
            OnClicked(context);
            Log("Beginning Drag");
        }

        protected void OnEndDrag(InputAction.CallbackContext context) {
            
            Log("Ending Drag");
        }
        
        protected override void Dispose() {
            base.Dispose();
            InputController.OnBeginDrag -= OnBeginDrag;
            InputController.OnEndDrag -= OnEndDrag;
        }

#endregion
        
    }
}
