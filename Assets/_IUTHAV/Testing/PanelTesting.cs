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
    private CinemachineFollow cmFollow;

    private bool panelIsActive;
    private ScrollRect scrollRect;
    private RectTransform _rectTransform;
    private float scrollAmount = 0;

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
    }

    private void Update()
    {
       MoveParalax();
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
        Vector2 newMousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        
        newMousePos = new Vector2(newMousePos.x - _rectTransform.anchoredPosition.x,
            newMousePos.y + _rectTransform.anchoredPosition.y);

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
