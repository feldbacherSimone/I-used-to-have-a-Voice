using _IUTHAV.Scripts.ComicPanel;
using UnityEngine;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.Dialogue.Option {
    public class BorderColour : MonoBehaviour{
    
        [SerializeField] private BorderColours borderColours;
        public void HighlightBorder() {
            gameObject.GetComponent<Image>().color = borderColours.borderHighlightColour;
        }

        public void ResetBorderColour() {
            gameObject.GetComponent<Image>().color = borderColours.borderBaseColour;
        }
    }
}