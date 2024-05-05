using _IUTHAV.Scripts.Interaction;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.Panel {
    public class Panel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool debug = true; 
    
        [SerializeField] private Vector3 defaultPos;
        [SerializeField] private float xPadding = 1;
        [SerializeField] private float yPadding = 1;
        [SerializeField] private float zPadding = 1;

        [SerializeField] private GameObject cmCamGameObject;
        [SerializeField] private Transform camTarget;
    
        public Camera panelCamera;
        private CinemachineFollow cmFollow;

        [HideInInspector] public bool isRendering;

        public bool panelIsActive;
        private ScrollRect scrollRect;
        private RectTransform _rectTransform;
        private float scrollAmount = 0;

        private Vector2 panelSize;

        private GameObject currentHitObject;

        private void Awake() {
        
            ConfigureCollider2D();
        
        }
        private void Start()
        {
            if (cmCamGameObject == null){
                DebugPrint("No cmCam assigned in PanelManager", true);
                return;
            }
            cmFollow = cmCamGameObject.GetComponent<CinemachineFollow>();
            defaultPos = camTarget.position; 
        
            _rectTransform = GetComponent<RectTransform>();
            scrollRect = GameObject.FindWithTag("Scroll").GetComponent<ScrollRect>();
            CameraMovement.InitProjection(cmCamGameObject.transform, camTarget.position);

            RawImage panelImage = GetComponent<RawImage>();
            panelSize = new Vector2(panelImage.texture.width, panelImage.texture.height);
        
            //Set rendering to false by default
            SetRendering(false);
        }

        private void Update()
        {
            MoveParalax();
            Raycast();
        }


        // ReSharper disable Unity.PerformanceAnalysis
        private void Raycast()
        {
            if(!panelIsActive)
                return;
            Vector3 realivePos = GetRelativeMousePos();
            Vector3 rayPos = new Vector3((realivePos.x + 0.5f), (1- (realivePos.y + 0.5f))  , 0);
        
            DebugPrint($"rayPos: {rayPos}");
            Ray screenRay = panelCamera.ViewportPointToRay(rayPos);

            RaycastHit hit;
            Debug.DrawRay(screenRay.origin, screenRay.direction * 1000, Color.red);
            
            if (Physics.Raycast(screenRay, out hit, 1000f))
            {
                Debug.DrawRay(screenRay.origin, screenRay.direction * 1000, Color.green);
                DebugPrint($"Raycast from {gameObject.name}, hit object: {hit.transform.gameObject.name}");
            
                if (currentHitObject != hit.transform.gameObject)
                {
                    if (currentHitObject != null) IterateSelectables(currentHitObject, false);
                    currentHitObject = hit.transform.gameObject;
                    IterateSelectables(currentHitObject, true);
                }
            }
            else if (currentHitObject != null)
            {
                IterateSelectables(currentHitObject, false);
                currentHitObject = null; 
            }

        }
        private void MoveParalax()
        {
            var resetPosition = CameraMovement.ResetCamera(camTarget, defaultPos);
            if (panelIsActive)
            {
                Vector3 relativeMousePos = GetRelativeMousePos();
                DebugPrint($"Current mouse position: {relativeMousePos}");

                scrollAmount = Mathf.Clamp(Input.mouseScrollDelta.y + scrollAmount, -zPadding, zPadding);

                Vector3 posDelta = new Vector3(
                    xPadding * -relativeMousePos.x, 
                    yPadding * -relativeMousePos.y, 
                    scrollAmount);

                camTarget.position = CameraMovement.GetMovementAmount(posDelta);
            }
            else if (resetPosition != null)
            {
                camTarget.position = (Vector3)resetPosition; 
            }
        }

        public Vector2 GetRelativeMousePos()
        {

            float canvasScroll = transform.parent.GetComponent<RectTransform>().anchoredPosition.y;
            DebugPrint($"Scroll Amount: {canvasScroll}");
        
            Vector2 newMousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            DebugPrint($"new Mouse pos: {newMousePos}");
        
            newMousePos = new Vector2(newMousePos.x - _rectTransform.anchoredPosition.x,
                newMousePos.y + _rectTransform.anchoredPosition.y + canvasScroll);

            Vector2 relativeMousePos = new Vector2(
                newMousePos.x / _rectTransform.rect.width - 0.5f,
                newMousePos.y / _rectTransform.rect.height - 0.5f);

            return relativeMousePos;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            scrollRect.enabled = false;
            panelIsActive = true;
        
            CameraMovement.InitProjection(cmCamGameObject.transform, camTarget.position);
        
            this.GetComponent<RawImage>().color = new Color(0.9f, 0.9f, 1f);
        
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            scrollRect.enabled = true;
            panelIsActive = false;
        
            GetComponent<RawImage>().color = Color.white;
            scrollAmount = 0;
        
            if (currentHitObject != null) IterateSelectables(currentHitObject, false);
            currentHitObject = null;
        }

        public void SetReferences(GameObject _cmCamGameObj, Transform _cmCamTarget, UnityEngine.Camera _panelCamera) { 
            cmCamGameObject = _cmCamGameObj;
            camTarget = _cmCamTarget;
            panelCamera = _panelCamera;
        }

        public void SetRendering(bool enable) {
        
            GetComponent<RawImage>().enabled = enable;
            if (panelCamera != null) panelCamera.enabled = enable;
            isRendering = enable;

        }
    
        private void ConfigureCollider2D() {
        
            BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
        
            Vector2 bounds = gameObject.GetComponent<RectTransform>().rect.size;
            coll.size = bounds;
            coll.offset = new Vector2(bounds.x / 2.0f, -bounds.y / 2.0f);
        }

        private void IterateSelectables(GameObject targetObj, bool enable) {

            foreach (ISelectable selectable in targetObj.GetComponents<ISelectable>()) {

                if (enable) {
                    
                    selectable.OnSelect(new SelectionContext(this));
                }
                else {
                    selectable.OnDeselect();
                }
                
            }

        }
        
        private void DebugPrint(string msg, bool error = false)
        {
            if(!debug) return;
        
            if(!error) Debug.Log(msg, this);
            else Debug.LogError(msg, this);
        }
    }
}
