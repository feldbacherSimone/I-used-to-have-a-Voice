using System;
using System.Net.Mime;
using _IUTHAV.Scripts.ComicPanel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.CustomUI {
    
    [RequireComponent(typeof(MediaTypeNames.Image))]
    public class ImageColourController : MonoBehaviour {
        
        [SerializeField] private BorderColours borderColours;

        [SerializeField] private TMP_Text text;
        
        [SerializeField] private Color highlightedTextColour = Color.white;

        private Color initialTextColor;

        private Image image;

        public void Start() {
            image = GetComponent<Image>();

            image.color = borderColours.borderBaseColour;

            if (text != null) {
                initialTextColor = text.color;
            }
        }

        public void SwitchToSelectionColour() {
            image.GetComponent<Image>().color = borderColours.borderSelectionColour;
            if (text != null) {
                text.color = initialTextColor;
            }
        }
        
        public void SwitchToHighlightColour() {
            image.GetComponent<Image>().color = borderColours.borderHighlightColour;
            if (text != null) {
                text.color = highlightedTextColour;
            }
        }
        
        public void ResetColour() {
            image.GetComponent<Image>().color = borderColours.borderBaseColour;
            if (text != null) {
                text.color = initialTextColor;
            }
        }
        
    }
}