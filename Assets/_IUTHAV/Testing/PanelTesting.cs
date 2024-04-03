using System;
using System.Collections;
using System.Collections.Generic;
using _IUTHAV.Testing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Cinemachine;
using UnityEditor;
using Object = System.Object;

public class PanelTesting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool debug = true; 
    
    [SerializeField] private Vector3 defaultPos;
    [SerializeField] private float xPadding = 1;
    [SerializeField] private float yPadding = 1;
    [SerializeField] private float zPadding = 1;

    [SerializeField] private GameObject cmCamGameObject;
    [SerializeField] private Transform camTarget;
    [SerializeField] private Camera panelCamera;
    private CinemachineFollow cmFollow;

    private bool panelIsActive;
    private ScrollRect scrollRect;
    private RectTransform _rectTransform;
    private float scrollAmount = 0;

    private Vector2 panelSize;

    private GameObject currentHitObject; 
    private void Start()
    {
        if (cmCamGameObject == null){
            DebugPrint("No cmCam assigned in PanelManager", true);
            return;
        }
        cmFollow = cmCamGameObject.GetComponent<CinemachineFollow>();
        defaultPos = camTarget.position; 
        
        _rectTransform = GetComponent<RectTransform>();
        scrollRect = transform.parent.parent.GetComponent<ScrollRect>();
        CameraMovement.InitProjection(cmCamGameObject.transform, camTarget.position);

        RawImage panelImage = GetComponent<RawImage>();
        panelSize = new Vector2(panelImage.texture.width, panelImage.texture.height);
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

        if (Physics.Raycast(screenRay, out hit, 1000f))
        {
            Debug.DrawRay(screenRay.origin, screenRay.direction * 1000, Color.green);
            DebugPrint($"Raycast from {gameObject.name}, hit object: {hit.transform.gameObject.name}");
            
            if (currentHitObject != hit.transform.gameObject)
            {
                currentHitObject?.GetComponent<ISelectable>()?.OnDeselect();
                currentHitObject = hit.transform.gameObject;
                currentHitObject.GetComponent<ISelectable>()?.OnSelect();
            }
        }
        else if (currentHitObject != null)
        {
            currentHitObject?.GetComponent<ISelectable>()?.OnDeselect();
            currentHitObject = null; 
        }

       
       
        Debug.DrawRay(screenRay.origin, screenRay.direction * 1000, Color.red);

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

    private Vector2 GetRelativeMousePos()
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
    }

    private void DebugPrint(string msg, bool error = false)
    {
        if(!debug) return;
        
        if(!error) Debug.Log(msg, this);
        else Debug.LogError(msg, this);
    }
}
