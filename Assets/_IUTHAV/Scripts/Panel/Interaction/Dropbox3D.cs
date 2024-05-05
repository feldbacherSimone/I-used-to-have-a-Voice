using UnityEngine;

namespace _IUTHAV.Scripts.Panel.Interaction {
    public class Dropbox3D : MonoBehaviour {
    
        [SerializeField] protected bool isDebug;

        protected DragAndSnapObject CurrentElement;
        protected bool IsFull;

        private void OnDestroy() {
            Dispose();
        }

        protected void OnCollisionEnter(Collision other) {
        
            if (other.gameObject.TryGetComponent(out DragAndSnapObject dropElement)) {

                if (dropElement.GetAssignedDropBox() == null || dropElement.GetAssignedDropBox() == this) {
                    
                    CurrentElement = dropElement;
                    CurrentElement.DropCallback += OnDropElementDropped;
                    Log("Something just collided with me... " + CurrentElement.gameObject.name);
                    
                }
                else {
                    Log("Wrong DropElement, ignoring it");
                }
                
            }
            
        }

        protected void OnCollisionExit(Collision other) {

            if (other.gameObject.TryGetComponent(out DragAndSnapObject dropElement)) {

                dropElement.DropCallback -= OnDropElementDropped;
                Log("Something left me... " + dropElement.gameObject.name);

            }
            
        }


#region Private Functions

        protected virtual void OnDropElementDropped(DragAndSnapObject dropElement) {

            if (IsFull) {
                dropElement.StartInvalidDropPointSequence();
            }
            else {
                IsFull = true;
                dropElement.SnapToTarget(transform.position, () => {
                dropElement.currentFlag = DragAndSnapObject.FLAG_LOCK;
                dropElement.StartValidDropPointSequence();
            });
            Log("You dropped something: " + dropElement.gameObject.name);
            }
            
        }

        protected void Dispose() {
            if (CurrentElement != null ) CurrentElement.DropCallback -= OnDropElementDropped;
        }

        protected void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[DropBox3D] " + msg);
        }
        
        protected void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[DropBox3D] " + msg);
        }

#endregion
    }
}
