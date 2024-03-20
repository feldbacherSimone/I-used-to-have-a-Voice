using System;
using System.Collections;
using System.Collections.Generic;
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
    private CinemachineFollow cmFollow;

    private bool panelIsActive;
    private ScrollRect scrollRect;
    private RectTransform _rectTransform;
    private float scrollAmount = 0; 
    [SerializeField] private float _returnSpeed = 2;

    private void Start()
    {
        if (cmCamGameObject == null){
            DebugPrint("No cmCam assigned in PanelManager", true);
        return;
        }

        _rectTransform = GetComponent<RectTransform>();
        scrollRect = transform.parent.parent.GetComponent<ScrollRect>();
        cmFollow = cmCamGameObject.GetComponent<CinemachineFollow>();
        defaultPos = cmFollow.FollowOffset; 
    }

    private void Update()
    {
        if (panelIsActive)
        {
            MoveParalax();
        }
        else if(Vector3.Distance(cmFollow.FollowOffset, defaultPos) > 0.1)
        {
            cmFollow.FollowOffset = Vector3.MoveTowards(cmFollow.FollowOffset, defaultPos, _returnSpeed * Time.deltaTime);
        }
    }

    private void MoveParalax()
    {
        Vector3 relativeMousePos = GetRelativeMousePos();
        DebugPrint($"Current mouse position: {relativeMousePos}");

        scrollAmount = Mathf.Clamp(Input.mouseScrollDelta.y + scrollAmount, -zPadding, zPadding);

        Vector3 posDelta = new Vector3(xPadding * relativeMousePos.x + defaultPos.x,
            yPadding * relativeMousePos.y + defaultPos.y, scrollAmount + defaultPos.z);

        cmFollow.FollowOffset = posDelta;
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
        if(!error) Debug.Log(msg, this);
        else Debug.LogError(msg, this);
    }
}
