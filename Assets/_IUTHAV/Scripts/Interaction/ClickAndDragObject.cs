using System.Collections.Generic;
using _IUTHAV.Scripts.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _IUTHAV.Scripts.Interaction {
    
    public class ClickAndDragObject : SelectAndClickObject {

        [SerializeField][Range(0, 40)] protected float throwSpeed = 20f;

        protected Panel.Panel OriginPanel;
        private Vector3 _mouseStartPosition;
        protected bool IsDrag;
        protected Rigidbody RBody;

        private Vector3 _mTargetPosition;

        protected VectorQueue VQueue;

#region Unity Functions

        private void OnDrawGizmos() {

            Gizmos.color = new Color(1, 0, 0, 0.3f);

            if (IsDrag) {
            
                Gizmos.DrawSphere(_mTargetPosition, 1f);
                Gizmos.color = new Color(1, 0.5f, 0, 1);

                if (VQueue.Count != 0) {
                    Vector3 velocity = CalculateVelocity();
                    Gizmos.DrawRay(_mTargetPosition, velocity);
                    Gizmos.DrawSphere(_mTargetPosition + velocity, 0.1f);
                }
                
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

        public override void OnSelect(SelectionContext context) {
        
            base.OnSelect(context);
            OriginPanel = context.Panel;

        }

#endregion

#region Private Functions

        private void UpdatePosition() {
            
            if (IsDrag) {

                _mTargetPosition = CalculatePosition();

                if (RBody != null) {
                    
                    VQueue.Enqueue(_mTargetPosition);
                    RBody.velocity = (_mTargetPosition - transform.position) * 10;
                    
                }

                transform.position = _mTargetPosition;
                
                //Stop when mouse leaves panel
                if (!OriginPanel.panelIsActive) {
                    OnEndDrag(new InputAction.CallbackContext());
                }
            }
            
        }

        protected override void Configure() {
        
            base.Configure();
            InputController.OnBeginDrag += OnBeginDrag;
            InputController.OnEndDrag += OnEndDrag;

            _mTargetPosition = transform.position;
            
            if (gameObject.TryGetComponent(out Rigidbody rb)) {

                RBody = rb;
                VQueue = new VectorQueue(10);
                
            }
            
        }

        protected virtual void OnBeginDrag(InputAction.CallbackContext context) {

            if (IsSelected) {
            
                _mouseStartPosition = Input.mousePosition - GetObjScreenPos();
                IsDrag = true;
                OnClicked(context);
                
                if (RBody != null) VQueue.Clear();

                Log("Beginning Drag");
                
            }
            
        }

        protected virtual void OnEndDrag(InputAction.CallbackContext context) {

            if (IsDrag) {
                IsDrag = false;
                _mouseStartPosition = Vector3.zero;

                if (RBody != null) {
                
                    RBody.velocity = CalculateVelocity();
                    
                }

                _mTargetPosition = Vector3.zero;
            }

        }

        private Vector3 CalculateVelocity() {
            
            Vector3 velocity = (_mTargetPosition - VQueue.GetAverage()) / VQueue.Count;
            return velocity * throwSpeed;
            
        }

        private Vector3 CalculatePosition() {
        
            if (OriginPanel != null) {

                Vector3 position =
                    OriginPanel.panelCamera.ScreenToWorldPoint(Input.mousePosition - _mouseStartPosition);
            
                if (RBody != null) {

                    position.x = (HasConstraint(RigidbodyConstraints.FreezePositionX)) ? transform.position.x : position.x;
                    position.y = (HasConstraint(RigidbodyConstraints.FreezePositionY)) ? transform.position.y : position.y;
                    position.z = (HasConstraint(RigidbodyConstraints.FreezePositionZ)) ? transform.position.z : position.z;

                }
                
                return position;
                
            }
            
            return transform.position;
        }
        
        protected override void Dispose() {
            base.Dispose();
            InputController.OnBeginDrag -= OnBeginDrag;
            InputController.OnEndDrag -= OnEndDrag;
        }
        
        protected bool HasConstraint(RigidbodyConstraints constraint) {
    		return (RBody.constraints & constraint) != RigidbodyConstraints.None;
    	}
        
        private Vector3 GetObjScreenPos() {

            if (OriginPanel != null) {
                return OriginPanel.panelCamera.WorldToScreenPoint(transform.position);
                
            }
            
            return transform.position;
        }

#endregion

#region Helper Classes
    
    protected class VectorQueue {

        private readonly Queue<Vector3> _queue;
        private readonly int _maxSize;
        
        public int Count {
            get {
                return _queue.Count;
            }
        }
        public VectorQueue(int maxSize){
            _maxSize = maxSize;
            _queue = new Queue<Vector3>();
        }
        public void Enqueue(Vector3 v){
            if(_queue.Count>=_maxSize){
                _queue.Dequeue();
            }
            _queue.Enqueue(v);
        }
        public Vector3 Peek(){
            return _queue.Peek();
        }

        public Vector3 GetAverage() {

            Vector3 result = Vector3.zero;
            foreach (Vector3 v in _queue) {

                result += v;

            }

            return result / _queue.Count;
        }
        public void Clear(){
            _queue.Clear();
        }
        
    }

#endregion
        
    }
}
