using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.ComicPanel.Interaction
{
    /// <summary>
    /// For Selecting 2D Objects on the canvas
    /// </summary>
    public class ImageSelectionResponse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image _image;
        private Sprite baseSprite;
        [SerializeField] private Sprite highlightSprite;

        private void Start()
        {
            _image = GetComponent<Image>();
            baseSprite = _image.sprite; 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _image.sprite = highlightSprite; 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _image.sprite = baseSprite; 
        }
    }
}