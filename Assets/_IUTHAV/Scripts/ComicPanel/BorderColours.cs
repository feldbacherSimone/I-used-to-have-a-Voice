using UnityEngine;

namespace _IUTHAV.Scripts.ComicPanel {

    [CreateAssetMenu(fileName = "BorderColours", menuName = "ScriptableObjects/BorderColours", order = 3)]
    public class BorderColours : ScriptableObject {
        
        public Color borderSelectionColour;
        public Color borderHighlightColour;
        public Color borderBaseColour;
        public Color errorColour;
        
    }
}