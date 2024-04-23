using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _IUTHAV.Scripts.CustomUI {
    public class DropBox : MonoBehaviour {


        [SerializeField] protected bool isDebug;
        
        void Start() {
            Configure();
        }

        protected void OnCollisionEnter2D(Collision2D other) {
        
            if (other.gameObject.TryGetComponent(out DragAndDropUIElement dropElement)) {

                dropElement.DropCallback += OnDropElementDropped;
                Log("Something just collided with me... " + dropElement.gameObject.name);
            }
            
        }

        protected void OnCollisionExit2D(Collision2D other) {
        
            if (other.gameObject.TryGetComponent(out DragAndDropUIElement dropElement)) {
                
                dropElement.DropCallback -= OnDropElementDropped;
                Log("Something left me... " + dropElement.gameObject.name);
            }
            
        }

        protected void OnDropElementDropped(DragAndDropUIElement dropElement, PointerEventData data) {
            
            dropElement.SnapToTarget(transform.position, () => {
                dropElement.currentflag = DragableUIElement.FLAG_LOCK;
            });
            Log("You dropped something: " + dropElement.gameObject.name);
        }
        
        protected void ConfigureCollider2D() {
            BoxCollider2D coll = gameObject.GetComponent<BoxCollider2D>();
            if (coll == null) {
                
                coll = gameObject.AddComponent<BoxCollider2D>();
                Rect rect= ((RectTransform)gameObject.transform).rect;
                Vector2 bounds = new Vector2(rect.width, rect.height);
                
                coll.size = bounds;
            }
        }
        
        protected void Configure() {
            ConfigureCollider2D();
        }

        protected void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[DropBox] " + msg);
        }
        
        protected void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[DropBox] " + msg);
        }
    }
}
