using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _IUTHAV.Scripts.CustomUI {
    public class DropBox : MonoBehaviour {

        [SerializeField] protected bool isDebug;

        [SerializeField] protected UnityEvent OnDragelementEnter;
        [SerializeField] protected UnityEvent OnDragelementExit;
        protected DragAndDropUIElement CurrentElement;
        protected bool IsFull;
        
        void Awake () {
            Configure();
        }

        public void Empty() {
            IsFull = false;
        }

        private void OnDestroy() {
            Dispose();
        }

        protected void OnCollisionEnter2D(Collision2D other) {
        
            if (other.gameObject.TryGetComponent(out DragAndDropUIElement dropElement)) {
                
                OnDragelementEnter.Invoke();
                CurrentElement = dropElement;
                CurrentElement.DropCallback += OnDropElementDropped;
                Log("Something just collided with me... " + CurrentElement.gameObject.name);
            }
            
        }

        protected void OnCollisionExit2D(Collision2D other) {
        
            if (other.gameObject.TryGetComponent(out DragAndDropUIElement dropElement)) {
                OnDragelementExit.Invoke();
                dropElement.DropCallback -= OnDropElementDropped;
                Log("Something left me... " + dropElement.gameObject.name);
            }
            
        }

        protected virtual void OnDropElementDropped(DragAndDropUIElement dropElement) {

            if (IsFull) {
                dropElement.StartInvalidDropPointSequence();
            }
            else {
                IsFull = true;
            dropElement.SnapToTarget(transform.position, () => {
                dropElement.currentflag = DragUIElement.FLAG_LOCK;
                dropElement.StartValidDropPointSequence();
                OnDragelementExit.Invoke();
            });
            Log("You dropped something: " + dropElement.gameObject.name);
            }
            
        }
        
        protected void ConfigureCollider2D() {
            BoxCollider2D coll = gameObject.GetComponent<BoxCollider2D>();
            if (coll == null) {
                
                coll = gameObject.AddComponent<BoxCollider2D>();
                Rect rect= ((RectTransform)gameObject.transform).rect;
                Vector2 bounds = new Vector2(rect.width, rect.height);
                
                coll.size = bounds;
            }
        }
        
        protected void Configure() {
            ConfigureCollider2D();
        }

        protected void Dispose() {
            if (CurrentElement != null ) CurrentElement.DropCallback -= OnDropElementDropped;
        }

        protected void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[DropBox] " + msg);
        }
        
        protected void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[DropBox] " + msg);
        }
    }
}
