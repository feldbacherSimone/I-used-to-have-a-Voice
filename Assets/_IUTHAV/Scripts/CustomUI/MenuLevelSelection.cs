using System;
using _IUTHAV.Scripts.Core.Audio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.CustomUI
{
    [Serializable]
    public class MenuLevelSelection : Button
    {
        public UnityEvent selectionAction; 
        public UnityEvent deselectionAction;

        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private Vector3 moveAmount;
        [SerializeField] private float animationSpeed = 1f;  // Speed of the animation
        [SerializeField] private Vector3 basePosition; 
        [SerializeField] private float animationTime;

        protected override void Awake()
        {
            base.Awake();
            onClick.AddListener(() => SoundManager.PlaySound(SoundManager.SoundType.UIClick, SoundManager.Mixer.SFX));
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            selectionAction.Invoke();
            SoundManager.PlaySound(SoundManager.SoundType.UIHover, SoundManager.Mixer.SFX);
        }

        
        private void Update()
        {
            // Oscillate between basePosition + moveAmount and basePosition - moveAmount
            animationTime += Time.deltaTime * animationSpeed;
            Vector3 offset = moveAmount * Mathf.Sin(animationTime);
            _rectTransform.anchoredPosition3D = basePosition + offset;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            deselectionAction.Invoke();
        }
    }
}