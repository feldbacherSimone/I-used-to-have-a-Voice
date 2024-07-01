using System.Collections;
using _IUTHAV.Scripts.Core.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _IUTHAV.Scripts.CustomUI {
    
    public class DragUIElement : SelectAndClickUIElement {
        
        [SerializeField] protected UnityEvent onPickup;
        [SerializeField] protected UnityEvent onDrop;
        
        public int currentflag = FLAG_NONE;
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
        
        private void Update() {

            if (currentflag == FLAG_DRAG) {
                ((RectTransform)transform).position = Input.mousePosition;
            }
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

        protected override void OnClickDelegate(BaseEventData data) {
            
            base.OnClickDelegate(data);
            
            if (InputController.IsHoldingElement) return;
        
            if (currentflag == FLAG_DRAG) {
                Drop(null);
            }

            if (currentflag != FLAG_NONE) return;
            
            onPickup.Invoke();

            CalculateCurrentPointerToCanvasPosition(((PointerEventData)data).position);

            if (Vector2.Distance(transform.position, _mTargetPosition) > 0.5f) {
                StartCoroutine(MoveTowardsTarget(() => { currentflag = FLAG_DRAG; }));
            }

            InputController.IsHoldingElement = true;
        }

        protected virtual void Drop(DragAndDropUIElement dropElement) {

            if (currentflag != FLAG_LOCK) {
                
                onDrop.Invoke();
                currentflag = FLAG_NONE;

                InputController.IsHoldingElement = false;
            }

        }

        protected void CalculateCurrentPointerToCanvasPosition(Vector2 pointerPosition) {
        
            // Only important, if cameraSpace is used!
            
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //    (RectTransform)canvas.transform,
            //    pointerPosition,
            //    canvas.worldCamera,
            //    out var position);
            
            _mTargetPosition = pointerPosition;
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
        
#endregion

        
    }
}
