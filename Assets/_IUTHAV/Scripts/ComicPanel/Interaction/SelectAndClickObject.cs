using _IUTHAV.Scripts.Core.Input;
using _IUTHAV.Scripts.CustomUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _IUTHAV.Scripts.ComicPanel.Interaction {
    public class SelectAndClickObject : MonoBehaviour, IInteractable, ISelectable {

        [Tooltip("Valid Panels, where Object can be selected. If none are selected, Object will be selectable everywhere")]
        [SerializeField] protected Panel[] validPanels;

        [SerializeField] protected UnityEvent onSelect;
        [SerializeField] protected UnityEvent onClick;
        [SerializeField] protected UnityEvent onDeselect;
        
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

            if (context.IsValidPanelExists(validPanels)) {
                IsSelected = true;
                CustomCursor.SetCursor(CursorState.Interact);
                onSelect?.Invoke();
            }
        }

        public virtual void OnDeselect() {
            IsSelected = false;
            CustomCursor.SetCursor(CursorState.Default);
            onDeselect?.Invoke();
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
