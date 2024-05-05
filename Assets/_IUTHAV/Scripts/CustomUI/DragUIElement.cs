using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _IUTHAV.Scripts.CustomUI {
    
    public class DragUIElement : MonoBehaviour {

        [SerializeField] protected Canvas canvas;
        
        [SerializeField] protected bool isDebug;

        [HideInInspector] public int currentflag = FLAG_NONE;
        public const int FLAG_LOCK = 3;
        public const int FLAG_DRAG = 1;
        public const int FLAG_NONE = 0;
        public const int FLAG_MOVESELF = 2;
        
        protected Vector2 _mTargetPosition;
        
        protected const float InitialSnapSpeed = 7.0f;

        public delegate void OnMoveCompleteDelegate();

#region Unity Functions

        private void Awake() {
            EventTrigger trigger = gameObject.GetOrAddComponent<EventTrigger>();
            Configure(trigger);
        }

        private void OnDestroy() {
            RemoveListeners();
        }

#endregion

#region Public Functions

        public void SnapToTarget(Vector2 target, OnMoveCompleteDelegate onMoveCompleteDelegate = null) {
            _mTargetPosition = target;
            StartCoroutine(MoveTowardsTarget(onMoveCompleteDelegate));
        }

#endregion

#region protected Functions

        protected virtual void OnBeginDragDelegate(BaseEventData data) {

            if (currentflag != FLAG_NONE) return;

            CalculateCurrentPointerToCanvasPosition(((PointerEventData)data).position);
            if (Vector2.Distance(transform.position, _mTargetPosition) > 0.5f) {
                StartCoroutine(MoveTowardsTarget(() => { currentflag = FLAG_DRAG; }));
            }
        }

        protected virtual void OnDragDelegate(BaseEventData data) {

            if (currentflag == FLAG_LOCK) return;
            
            PointerEventData pointerData = (PointerEventData)data;
            CalculateCurrentPointerToCanvasPosition(pointerData.position);

            if (currentflag != FLAG_MOVESELF) {
                ((RectTransform)transform).position = new Vector3(_mTargetPosition.x, _mTargetPosition.y, transform.position.z);
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
            
            if (canvas == null) {
                canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
            }
            
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
            float speed = InitialSnapSpeed;
            while (Vector2.Distance(transform.position, _mTargetPosition) > 0.1f) {
                speed *= acceleration;
                
                Vector2 target = Vector2.MoveTowards(((RectTransform)transform).position, _mTargetPosition, speed);

                ((RectTransform)transform).position = new Vector3(target.x, target.y, transform.position.z);
                    
                yield return null;
            }
            
            currentflag = FLAG_NONE;
            if (onFinishMove != null) onFinishMove();
        }
        
        protected void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[DragableUIElement] [" + gameObject.name + "] " + msg);
        }
        
        protected void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[DragableUIElement] [" + gameObject.name + "] " + msg);
        }
        
#endregion

        
    }
}
