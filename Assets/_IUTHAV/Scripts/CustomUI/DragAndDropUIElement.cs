using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.CustomUI {
    public class DragAndDropUIElement : DragableUIElement {

        [SerializeField] protected UnityEvent onValidDropSequence;
        [SerializeField] protected UnityEvent onInvalidDropSequence;
        [SerializeField] protected UnityEvent onDestructionSequence;
        public delegate void DropDelegate(DragAndDropUIElement dragAndDropUIElement, PointerEventData data);
        public DropDelegate DropCallback;
        protected Vector2 startingPosition;
        
        private void Awake() {
            EventTrigger trigger = gameObject.GetOrAddComponent<EventTrigger>();
            Configure(trigger);
        }

        public virtual void StartValidDropPointSequence() {
            Log("Starting ValidDrop Sequence");
            onValidDropSequence?.Invoke();
        }

        public virtual void StartInvalidDropPointSequence() {
            Log("Starting InvalidDrop Sequence");
            onInvalidDropSequence?.Invoke();
        }

        public virtual void StartDestructionSequence() {
            Log("Starting Destruction Sequence");
            onDestructionSequence?.Invoke();
        }

        protected override void OnBeginDragDelegate(BaseEventData data) {
            startingPosition = transform.position;
            base.OnBeginDragDelegate(data);
            
        }

        protected override void OnDropDelegate(BaseEventData data) {
            base.OnDropDelegate(data);
            SnapToTarget(startingPosition);
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

            startingPosition = transform.position;

        }

    }
}
