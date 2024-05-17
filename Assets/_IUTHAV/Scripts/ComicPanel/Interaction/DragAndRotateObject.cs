using UnityEngine;
using UnityEngine.InputSystem;

namespace _IUTHAV.Scripts.ComicPanel.Interaction {
    public class DragAndRotateObject : ClickAndDragObject {

        [SerializeField][Range(0, 5f)] private float rotateSpeed = 1f;

        private Vector3 _startRotation;
        private Vector3 _mouseScreenStartPosition;

#region Unity Functions

        protected override void OnDrawGizmos() {

            Gizmos.color = new Color(1, 0, 0, 0.3f);

            if (currentFlag == FLAG_DRAG) {
            
                Gizmos.DrawSphere(transform.position, 1f);
                Gizmos.color = new Color(1, 0.5f, 0, 1);

                if (VQueue.Count != 0) {
                    
                    Vector3 velocity = CalculateVelocity();
                    Gizmos.DrawWireSphere(transform.position, velocity.magnitude / 10.0f);
                }
                
            }
            
        }
        private void OnEnable() {
            Configure();
        }

        private void Update() {

            UpdateRotation();

        }

        private void OnDisable() {
            Dispose();
        }

#endregion

#region Public Functions

        public override void OnSelect(SelectionContext context) {
        
            base.OnSelect(context);
            if (OriginPanel == null) OriginPanel = context.Panel;

        }

#endregion

#region Private Functions

        protected override void OnBeginDrag(InputAction.CallbackContext context) {

            if (IsSelected) {
                
                base.OnBeginDrag(context);
                
                _startRotation = transform.rotation.eulerAngles;
                _mouseScreenStartPosition = Input.mousePosition;

                if (RBody != null) {
                    
                    RBody.angularVelocity = Vector3.zero;
                    
                }
                
            }
            
        }

        protected override void OnEndDrag(InputAction.CallbackContext context) {
        
            if (currentFlag == FLAG_DRAG) {
                currentFlag = FLAG_NONE;
                _mouseScreenStartPosition = Vector3.zero;

                if (RBody != null) {
                
                    RBody.AddTorque(CalculateVelocity());
                    
                    VQueue.Clear();
                    
                }

                OriginPanel = null;
            }
            
            
        }
        
        private Vector3 CalculateVelocity() {
            
            //Log(transform.rotation.eulerAngles + " | " + VQueue.Peek().ToString()  );
            Vector3 velocity = (transform.rotation.eulerAngles - VQueue.GetAverage());
            return velocity;
            
        }

        private void UpdateRotation() {

            if (currentFlag == FLAG_DRAG) {
                
                Vector3 rotation = CalculateRotationDelta() * rotateSpeed;
                
                Log(rotation.ToString());
                
                transform.rotation = Quaternion.Euler(_startRotation + rotation);
                
                if (RBody != null) {
                    
                    VQueue.Enqueue(transform.rotation.eulerAngles);
                    
                }
                
            }
            
        }

        private Vector3 CalculateRotationDelta() {
            
            //Flip x and y!
            //Rotate only by 2 axis because simple and mouseinput only offers 2 floats
            float deltaX = (Input.mousePosition.y - _mouseScreenStartPosition.y) * -1;
            float deltaY = (Input.mousePosition.x - _mouseScreenStartPosition.x) * -1;
            float deltaZ = Input.mousePosition.z - _mouseScreenStartPosition.z;

            if (RBody != null) {

                deltaX = (HasConstraint(RigidbodyConstraints.FreezeRotationX)) ? 0 : deltaX;
                deltaY = (HasConstraint(RigidbodyConstraints.FreezeRotationY)) ? 0 : deltaY;
                deltaZ = (HasConstraint(RigidbodyConstraints.FreezeRotationZ)) ? 0 : deltaZ;

            }

            return new Vector3(deltaX, deltaY, deltaZ);
        }
        
        

#endregion


    }
}
