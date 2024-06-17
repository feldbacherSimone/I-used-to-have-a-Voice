using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.CustomUI
{
    [Serializable]
    public class MenuButton : Button, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent   selectionAction; 
        public UnityEvent  deselectionAction;

        [SerializeField] public int aaaa = 1; 
        public void OnPointerEnter(PointerEventData eventData)
        {
            selectionAction.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
           deselectionAction.Invoke();
        }
    }
}