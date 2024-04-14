using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Dialogue {
    public class CharacterBox : MonoBehaviour {
    
        private RectTransform _boxRectTransform;
        [SerializeField] private bool isRightAlignment;
        
        public RectTransform BoxRectTransform => _boxRectTransform;
        public bool IsRightAlignment => isRightAlignment;

        private void Awake() {
        
            RectTransform rectTransform = GetComponent<RectTransform>();

            _boxRectTransform = rectTransform;
        }
    }
}
