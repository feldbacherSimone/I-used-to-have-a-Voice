
using _IUTHAV.Scripts.ComicPanel.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.ComicPanel {
    public class Panel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool debug = true;
        
        [Space(20)]
        [SerializeField] private bool updateRenderTextureSize;
        [Space(20)]
        
        [SerializeField] private Vector3 defaultPos;
        [SerializeField] private Vector3 movementPadding;

        [SerializeField] private GameObject cmCamGameObject;
        [SerializeField] private Transform camTarget;
        public Camera panelCamera;
        [HideInInspector] public bool isRendering;
        public bool panelIsActive;
        
        private ScrollRect scrollRect;
        private RectTransform backgroundTransform;
        private RectTransform rectTransform;
        private GameObject currentHitObject;


#region Unity Functions
        private void OnValidate() {

            if (updateRenderTextureSize) {
                updateRenderTextureSize = false;
            
                UpdateRenderTexture();
            }
            
        }
        private void Awake() {
        
            ConfigureCollider2D();

        }
        private void Start()
        {
            if (cmCamGameObject == null){
                DebugPrint("No cmCam assigned in PanelManager", true);
                return;
            }
            
            //Init Camera Movement
            defaultPos = camTarget.position;
            rectTransform = GetComponent<RectTransform>();
            CameraMovement.InitProjection(cmCamGameObject.transform, camTarget.position);

            //ScrollRect Configuration
            scrollRect = GameObject.FindWithTag("Scroll").GetComponent<ScrollRect>();
            backgroundTransform = GameObject.FindWithTag("BG").GetComponent<RectTransform>();
        
            //Set rendering to false by default
            SetRendering(false);
            
        }

        private void Update()
        {
            MoveParalax();
            if (panelIsActive) Raycast();
        }


#endregion

#region Public Functions
        // ReSharper disable Unity.PerformanceAnalysis
        public Vector2 GetRelativeMousePos()
        {
            float canvasScroll = backgroundTransform.anchoredPosition.y;
            DebugPrint($"Scroll Amount: {canvasScroll}");
                
            Vector2 scrollIndependantMousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            DebugPrint($"Mouse Pos with scroll pos: {scrollIndependantMousePos}");
                
            Vector2 mousePosOnPanel = new Vector2(scrollIndependantMousePos.x - rectTransform.anchoredPosition.x,
                scrollIndependantMousePos.y + rectTransform.anchoredPosition.y + canvasScroll);

            Vector2 relativeMousePos = new Vector2(
                mousePosOnPanel.x / rectTransform.rect.width - 0.5f,
                mousePosOnPanel.y / rectTransform.rect.height - 0.5f);
            
            DebugPrint($"Relative Mouse Pos: {relativeMousePos}");
            return relativeMousePos;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            scrollRect.enabled = false;
            panelIsActive = true;
        
            CameraMovement.InitProjection(cmCamGameObject.transform, camTarget.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            scrollRect.enabled = true;
            panelIsActive = false;

            if (currentHitObject != null) IterateSelectableComponents(currentHitObject, false);
            currentHitObject = null;
        }

        public void SetReferences(GameObject _cmCamGameObj, Transform _cmCamTarget, Camera _panelCamera) { 
            cmCamGameObject = _cmCamGameObj;
            camTarget = _cmCamTarget;
            panelCamera = _panelCamera;
        }

        public void SetRendering(bool enable) {
            
            if (enable) {
                GetComponent<RawImage>().enabled = true;
                RenderTexture tex = (RenderTexture)GetComponent<RawImage>().texture;
                tex.Create();
                
            }
            else {
                RenderTexture tex = (RenderTexture)GetComponent<RawImage>().texture;
                tex.Release();
                GetComponent<RawImage>().enabled = false;
                
            }
            
            if (panelCamera != null) panelCamera.enabled = enable;
            isRendering = enable;

        }
#endregion

#region Private Functions
// ReSharper disable Unity.PerformanceAnalysis
        private void Raycast()
        {
            
            //init Ray
            Vector3 relativeMousePos = GetRelativeMousePos();
            Vector3 rayPos = new Vector3(
                (relativeMousePos.x + 0.5f), 
                (1- (relativeMousePos.y + 0.5f)), 
                0);
            
            Ray screenRay = panelCamera.ViewportPointToRay(rayPos);
            RaycastHit hit;
            Debug.DrawRay(screenRay.origin, screenRay.direction * 1000, Color.red);
                    
            if (Physics.Raycast(screenRay, out hit, 1000f))
            {
                Debug.DrawRay(screenRay.origin, screenRay.direction * 1000, Color.green);
                DebugPrint($"Raycast from {gameObject.name}, hit object: {hit.transform.gameObject.name}");
                    
                if (currentHitObject != hit.transform.gameObject)
                {
                    if (currentHitObject != null) IterateSelectableComponents(currentHitObject, false);
                    currentHitObject = hit.transform.gameObject;
                    IterateSelectableComponents(currentHitObject, true);
                }
            }
            else if (currentHitObject != null)
            {
                IterateSelectableComponents(currentHitObject, false);
                currentHitObject = null; 
            }
        }
        private void MoveParalax()
        {
            var resetPosition = CameraMovement.ResetCamera(camTarget, defaultPos);
            
            if (panelIsActive)
            {
                Vector3 relativeMousePos = GetRelativeMousePos();

                Vector3 posDelta = new Vector3(
                    movementPadding.x * relativeMousePos.x, 
                    movementPadding.y * -relativeMousePos.y, 
                    0);

                camTarget.position = CameraMovement.GetMovementAmount(posDelta);
            }
            else if (resetPosition != null)
            {
                camTarget.position = (Vector3)resetPosition; 
            }
        }
        private void ConfigureCollider2D() {
        
            BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
        
            Vector2 bounds = gameObject.GetComponent<RectTransform>().rect.size;
            coll.size = bounds;
            coll.offset = new Vector2(bounds.x / 2.0f, -bounds.y / 2.0f);
        }

        private void IterateSelectableComponents(GameObject targetObj, bool enable) {

            foreach (ISelectable selectable in targetObj.GetComponents<ISelectable>()) {
                
                if (enable) {
                    selectable.OnSelect(new SelectionContext(this));
                }
                else {
                    selectable.OnDeselect();
                }
            }
        }

        private void UpdateRenderTexture() {
            
            var img = gameObject.GetComponent<RawImage>();
            RenderTexture tex = (RenderTexture)img.texture;
            Rect rec = gameObject.GetComponent<RectTransform>().rect;
            
            int width = (int)rec.width;
            int height = (int)rec.height;
            gameObject.GetComponent<RectTransform>().rect.Set(rec.x, rec.y, width, height);
            
            //thank you @Mandoz on discussions.unity.com
            tex.Release();
            tex.width = width;
            tex.height = height;
            tex.Create();

        }

        private void DebugPrint(string msg, bool error = false)
        {
            if(!debug) return;
        
            if(!error) Debug.Log(msg, this);
            else Debug.LogError(msg, this);
        }
#endregion
    }
}
