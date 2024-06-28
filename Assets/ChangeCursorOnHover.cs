using System.Collections;
using System.Collections.Generic;
using _IUTHAV.Scripts.CustomUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CursorState _cursorState;
    public void OnPointerEnter(PointerEventData eventData)
    {
        CustomCursor.SetCursor(_cursorState);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CustomCursor.SetCursor(CursorState.Default);
    }
}
