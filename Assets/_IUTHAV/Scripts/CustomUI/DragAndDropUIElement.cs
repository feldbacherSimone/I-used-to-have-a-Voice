using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _IUTHAV.Scripts.CustomUI {
    public class DragAndDropUIElement : DragUIElement {

        [SerializeField] protected UnityEvent onValidDrop;
        [SerializeField] protected UnityEvent onInvalidDrop;
        [SerializeField] protected UnityEvent onDestruction;
        public delegate void DropDelegate(DragAndDropUIElement dragAndDropUIElement, PointerEventData data);
        public DropDelegate DropCallback;
        protected Vector2 StartingPosition;
        
        private void Awake() {
            EventTrigger trigger = gameObject.GetOrAddComponent<EventTrigger>();
            Configure(trigger);
        }

        public virtual void StartValidDropPointSequence() {
            Log("Starting ValidDrop Sequence");
            onValidDrop?.Invoke();
        }

        public virtual void StartInvalidDropPointSequence() {
            Log("Starting InvalidDrop Sequence");
            onInvalidDrop?.Invoke();
        }

        public virtual void StartDestructionSequence() {
            Log("Starting Destruction Sequence");
            onDestruction?.Invoke();
        }

        protected override void OnBeginDragDelegate(BaseEventData data) {
            StartingPosition = transform.position;
            base.OnBeginDragDelegate(data);
            
        }

        protected override void OnDropDelegate(BaseEventData data) {
            base.OnDropDelegate(data);
            SnapToTarget(StartingPosition);
            DropCallback?.Invoke(this, (PointerEventData)data);
        }

        protected void ConfigureCollider2D() {
            BoxCollider2D coll = gameObject.GetComponent<BoxCollider2D>();
            if (coll == null) {
                
                coll = gameObject.AddComponent<BoxCollider2D>();
                Rect rect= ((RectTransform)gameObject.transform).rect;
                Vector2 bounds = new Vector2(rect.width, rect.height);

                coll.size = bounds;
                
            }

            Rigidbody2D rig = gameObject.GetComponent<Rigidbody2D>();
            if (rig == null) {
                rig = gameObject.AddComponent<Rigidbody2D>();
                rig.gravityScale = 0;
                rig.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        protected new void Configure(EventTrigger trigger) {
            
            base.Configure(trigger);

            ConfigureCollider2D();

            StartingPosition = transform.position;

        }

    }
}
