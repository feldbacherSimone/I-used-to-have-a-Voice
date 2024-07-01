using System;
using UnityEngine;

namespace _IUTHAV.Scripts.ComicPanel.Interaction
{
    /// <summary>
    /// For Selecting 2D Objects in the 3D space
    /// need a box collider! Either as a child or component -> Check prefabs/2D Interactable/Collider
    /// </summary>
    public class SpriteSelectionResponse : MonoBehaviour, ISelectable
    {
        [SerializeField] protected Panel[] validPanels;
        [SerializeField] private Sprite outlineSprite;
        [SerializeField] private Sprite baseSprite;

        private SpriteRenderer _spriteRenderer; 
        private void Awake()
        {
            if(!TryGetComponent<SpriteRenderer>(out _spriteRenderer))
                _spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
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