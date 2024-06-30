using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _IUTHAV.Scripts.CustomUI {
    public class SelectAndClickUIElement : MonoBehaviour{
        
        [SerializeField] protected Canvas canvas;
        
        [SerializeField] protected UnityEvent onPointerEnter;
        [SerializeField] protected UnityEvent onPointerExit;
        [SerializeField] protected UnityEvent onClick;
        
        [SerializeField] protected bool isDebug;
        
#region Unity Functions

        private void Awake() {
            EventTrigger trigger = gameObject.GetOrAddComponent<EventTrigger>();
            Configure(trigger);
        }

        private void OnDestroy() {
            RemoveListeners();
        }

        

#endregion
        protected virtual void OnClickDelegate(BaseEventData data) {
            
            onClick.Invoke();
        }
        
        protected void Configure(EventTrigger trigger) {
            
            EventTrigger.Entry entry = new EventTrigger.Entry();
            
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;

            entry.callback.AddListener((data) => { OnClickDelegate((PointerEventData)data);});
            trigger.triggers.Add(entry);
            
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;

            entry.callback.AddListener((data) => { onPointerEnter.Invoke();});
            trigger.triggers.Add(entry);
            
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;

            entry.callback.AddListener((data) => { onPointerExit.Invoke();});
            trigger.triggers.Add(entry);
            
            if (canvas == null) {
                canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
            }
            
        }

        protected void RemoveListeners() {
        
            if (TryGetComponent(out EventTrigger trigger)) {
                trigger.triggers.Clear();
            }
            
        }
        
        protected void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[DragableUIElement] [" + gameObject.name + "] " + msg);
        }
        
        protected void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[DragableUIElement] [" + gameObject.name + "] " + msg);
        }
    }
}