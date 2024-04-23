using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.CustomUI {
    
    public class DragableUIElement : MonoBehaviour {

        [SerializeField] protected Canvas canvas;
        public float initialSnapSpeed = 10.0f;
        public string currentflag = FLAG_NONE;
        public const string FLAG_LOCK = "FLAG_LOCK";
        public const string FLAG_DRAG = "FLAG_DRAG";
        public const string FLAG_NONE = "FLAG_NONE";
        public const string FLAG_MOVESELF = "FLAG_MOVESELF";

        protected Vector2 _mTargetPosition;

        public delegate void OnMoveCompleteDelegate();

#region Unity Functions

        private void Start() {
            EventTrigger trigger = gameObject.GetOrAddComponent<EventTrigger>();
            Configure(trigger);
        }

        private void OnDestroy() {
            RemoveListeners();
        }

#endregion

#region protected Functions

        public void SnapToTarget(Vector2 target, OnMoveCompleteDelegate onMoveCompleteDelegate = null) {
            _mTargetPosition = target;
            StartCoroutine(MoveTowardsTarget(onMoveCompleteDelegate));
        }

        public virtual void OnBeginDragDelegate(BaseEventData data) {

            if (currentflag != FLAG_NONE) return;
            
            CalculateCurrentPointerToCanvasPosition(((PointerEventData)data).position);
            if (Vector2.Distance(transform.position, _mTargetPosition) > 0.5f) {
                StartCoroutine(MoveTowardsTarget(() => { currentflag = FLAG_DRAG; }));
            }
        }

        public virtual void OnDragDelegate(BaseEventData data) {

            if (currentflag == FLAG_LOCK) return;
            
            PointerEventData pointerData = (PointerEventData)data;
            CalculateCurrentPointerToCanvasPosition(pointerData.position);

            if (currentflag != FLAG_MOVESELF) {
                transform.position = _mTargetPosition;
            }
            
        }

        protected virtual void OnDropDelegate(BaseEventData data) {

            if (currentflag != FLAG_LOCK) currentflag = FLAG_NONE;

        }

        protected void Configure(EventTrigger trigger) {
            
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;

            entry.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data);});
            trigger.triggers.Add(entry);
            
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;

            entry.callback.AddListener((data) => { OnBeginDragDelegate((PointerEventData)data);});
            trigger.triggers.Add(entry);
            
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;

            entry.callback.AddListener((data) => { OnDropDelegate((PointerEventData)data);});
            trigger.triggers.Add(entry);
            
        }

        protected void RemoveListeners() {
        
            if (TryGetComponent(out EventTrigger trigger)) {
                trigger.triggers.Clear();
            }
            
        }

        protected void CalculateCurrentPointerToCanvasPosition(Vector2 pointerPosition) {
        
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform,
                pointerPosition,
                canvas.worldCamera,
                out var position);

            _mTargetPosition = canvas.transform.TransformPoint(position);
        }

        protected IEnumerator MoveTowardsTarget(OnMoveCompleteDelegate onFinishMove = null) {
            
            currentflag = FLAG_MOVESELF;

            float acceleration = 1.1f;
            float speed = initialSnapSpeed;
            while (Vector2.Distance(transform.position, _mTargetPosition) > 0.1f) {
                speed *= acceleration;
                transform.position =
                    Vector3.MoveTowards(transform.position, _mTargetPosition, speed);
                yield return null;
            }
            
            currentflag = FLAG_NONE;
            if (onFinishMove != null) onFinishMove();
        }
        
#endregion
        
    }
}
