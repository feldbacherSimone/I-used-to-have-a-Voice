using System;
using UnityEngine;

namespace _IUTHAV.Scripts.ComicPanel.Interaction
{
    public class SpriteSelectionResponse : MonoBehaviour, ISelectable
    {
        [SerializeField] protected Panel[] validPanels;
        [SerializeField] private Sprite outlineSprite;
        [SerializeField] private Sprite baseSprite;

        private SpriteRenderer _spriteRenderer; 
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (!baseSprite) baseSprite = _spriteRenderer.sprite;
        }

        public void OnSelect(SelectionContext context)
        {
            if (context.IsValidPanelExists(validPanels)) {
                _spriteRenderer.sprite = outlineSprite;
            }
        }

        public void OnDeselect()
        {
            _spriteRenderer.sprite = baseSprite; 
        }
    }
}