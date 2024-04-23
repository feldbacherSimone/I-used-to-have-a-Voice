using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _IUTHAV.Scripts.CustomUI {
    public class DragAndDropUIElement : DragableUIElement {
        
        public delegate void DropDelegate(DragAndDropUIElement dragAndDropUIElement, PointerEventData data);
        public DropDelegate DropCallback;
        protected Vector2 startingPosition;
        
        private void Start() {
            EventTrigger trigger = gameObject.GetOrAddComponent<EventTrigger>();
            Configure(trigger);
        }

        protected override void OnDropDelegate(BaseEventData data) {
            base.OnDropDelegate(data);
            SnapToTarget(startingPosition);
            DropCallback?.Invoke(this, (PointerEventData)data);
        }
        
        public override void OnDragDelegate(BaseEventData data) {
        
            base.OnDragDelegate(data);
            Debug.Log("Doing more specific stuff");

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
