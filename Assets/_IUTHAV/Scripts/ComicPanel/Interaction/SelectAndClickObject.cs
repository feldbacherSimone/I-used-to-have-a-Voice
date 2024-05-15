using _IUTHAV.Scripts.Core.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _IUTHAV.Scripts.ComicPanel.Interaction {
    public class SelectAndClickObject : MonoBehaviour, IInteractable, ISelectable {
        
        
        [SerializeField] protected UnityEvent onClick;
        
        [SerializeField] protected bool isDebug;
        
        protected bool IsSelected;

#region Unity Functions

        private void OnEnable() {
            Configure();
        }

        private void OnDisable() {
            Dispose();
        }

#endregion

#region Public Functions

        public virtual void Interact() {

            onClick.Invoke();
            Log("Interacted with Object!");
        }
        public virtual void OnSelect(SelectionContext context) {
            IsSelected = true;
        }

        public virtual void OnDeselect() {
            IsSelected = false;
        }

#endregion
        
        
        protected virtual void Configure() {
            
            //Subscribe to Custom Click instead of making an Event Trigger, since the panels
            //would block the interaction
            InputController.OnCustomClick += OnClicked;

        }

        protected void OnClicked(InputAction.CallbackContext context) {
            
            if (IsSelected) {
                Interact();
            }
            
        }

        protected virtual void Dispose() {
            
            InputController.OnCustomClick -= OnClicked;
            
        }
        
        protected void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[Select&ClickObject] [" + gameObject.name + "] " + msg);
        }
        
        protected void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[Select&ClickObject] [" + gameObject.name + "] " + msg);
        }
    }
}
