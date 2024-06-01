using Unity.VisualScripting;
using UnityEngine;

namespace _IUTHAV.Scripts.ComicPanel {
    public class PanelManager : MonoBehaviour {

        [SerializeField][Range(0.5f, 2f)] private float renderBoundsFactor = 1.2f;
        [SerializeField] private bool isDebug;

        private Vector2 _areaSize;
#region Unity Functions

        private void Start() {
        
            Rect canvasRect = GameObject.FindWithTag("MainCanvas").GetComponent<RectTransform>().rect;
            _areaSize = canvasRect.size * renderBoundsFactor;

            Configure();
            CheckDormantColliders();
        }

        private void OnTriggerEnter2D(Collider2D other) {

            if (other.gameObject.TryGetComponent(out ComicPanel.Panel panel)) {

                if (!panel.isRendering) {
                    panel.SetRendering(true);
                    Log("Enabling panel: " + other.gameObject.name);
                }
                
            }
            
        }

        private void OnTriggerExit2D(Collider2D other) {
            
            if (other.gameObject.TryGetComponent(out ComicPanel.Panel panel)) {

                if (panel.isRendering) {
                    panel.SetRendering(false);
                    Log("Disabling panel: " + other.gameObject.name);
                }
                
            }
            
        }

#endregion

#region Public Functions

        

#endregion

#region Private Functions

        private void Configure() {

            BoxCollider2D collider2D = gameObject.GetOrAddComponent<BoxCollider2D>();

            collider2D.size = _areaSize;
            
            Rigidbody2D rig = gameObject.GetComponent<Rigidbody2D>();
            if (rig == null) {
                rig = gameObject.AddComponent<Rigidbody2D>();
                rig.gravityScale = 0;
                rig.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            
        }

        private void CheckDormantColliders() {
            
            Collider2D[] collider2Ds =
                Physics2D.OverlapBoxAll(
                transform.position, 
                     _areaSize / 10.0f, 
                    0f
                );

            foreach (Collider2D other in collider2Ds) {

                if (other.gameObject.TryGetComponent(out ComicPanel.Panel panel)) {
                    
                    if (!panel.isRendering) panel.SetRendering(true);
                    Log("Enabling panel: " + other.gameObject.name);
                }
                
            }
            
        }
        
        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[PanelManager] " + msg);
        }
        
        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[PanelManager] " + msg);
        }

#endregion

    }
}
