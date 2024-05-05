using System.Collections;
using _IUTHAV.Scripts.CustomUI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _IUTHAV.Scripts.Panel.Interaction {
    public class DragAndSnapObject : ClickAndDragObject {

        [Tooltip("If a droplocation is assigned, only that one will be snapped to")]
        [CanBeNull] private Dropbox3D _assignedDropLocation;

        private Vector3 _objectStartLocalPos;
        
        [SerializeField] protected UnityEvent onValidDrop;
        [SerializeField] protected UnityEvent onInvalidDrop;
        
        public const short FLAG_LOCK = 3;
        public const short FLAG_MOVESELF = 2;
        
        private Vector3 _mTargetObjectPosition;
        
        private const float InitialSnapSpeed = 0.01f;

        public delegate void DropDelegate3D(DragAndSnapObject dragAndSnapObject);
        public DropDelegate3D DropCallback;


#region Unity Functions

        private void Awake() {
        
            _objectStartLocalPos = transform.localPosition;
            
        }

        protected override void OnDrawGizmos() {

            base.OnDrawGizmos();

            if (currentFlag == FLAG_DRAG) {
                
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Gizmos.DrawSphere(_objectStartLocalPos, 1f);
                
            }
            
        }

        private void OnEnable() {
            Configure();
        }

        private void Update() {

            UpdatePosition();

        }

        private void OnDisable() {
            Dispose();
        }

#endregion

#region Public Functions

        [CanBeNull]
        public Dropbox3D GetAssignedDropBox() {
            return _assignedDropLocation;
        }

        public virtual void StartValidDropPointSequence() {
            Log("Starting ValidDrop Sequence");
            RBody.constraints |= RigidbodyConstraints.FreezePosition;
            RBody.useGravity = false;
            onValidDrop?.Invoke();
        }

        public virtual void StartInvalidDropPointSequence() {
            Log("Starting InvalidDrop Sequence");
            onInvalidDrop?.Invoke();
        }
        
        public void SnapToTarget(Vector3 target, DragUIElement.OnMoveCompleteDelegate onMoveCompleteDelegate = null) {
            _mTargetObjectPosition = target;
            StartCoroutine(MoveTowardsTarget(onMoveCompleteDelegate));
        }

#endregion

#region Private Functions

        protected override void OnBeginDrag(InputAction.CallbackContext context) {

            if (IsSelected && currentFlag == FLAG_NONE) {

                base.OnBeginDrag(context);
                currentFlag = FLAG_DRAG;

            }
            
        }

        protected override void OnEndDrag(InputAction.CallbackContext context) {

            if (currentFlag != FLAG_LOCK && currentFlag == FLAG_DRAG) {
                base.OnEndDrag(context);
                OnDrop();
            }
            
        }
        
        protected void OnDrop() {
            SnapToTarget(_objectStartLocalPos);
            DropCallback?.Invoke(this);
        }
        
        protected IEnumerator MoveTowardsTarget(DragUIElement.OnMoveCompleteDelegate onFinishMove = null) {
            
            currentFlag = FLAG_MOVESELF;

            float acceleration = 1.1f;
            float speed = InitialSnapSpeed;
            while (Vector3.Distance(transform.localPosition, _mTargetObjectPosition) > 0.1f) {
                speed *= acceleration;
                
                Vector3 target = Vector3.MoveTowards(transform.localPosition, _mTargetObjectPosition, speed);

                transform.localPosition = target;
                    
                yield return null;
            }
            
            currentFlag = FLAG_NONE;
            if (onFinishMove != null) onFinishMove();
        }

        

#endregion
        
    }
}
