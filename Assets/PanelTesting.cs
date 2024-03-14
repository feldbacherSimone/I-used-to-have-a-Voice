using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelTesting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        this.GetComponent<RawImage>().color = Color.blue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<RawImage>().color = Color.white;
    }
}
