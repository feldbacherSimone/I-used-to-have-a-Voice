using System;
using _IUTHAV.Scripts.CustomUI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Dialogue.Option {
    public class DragableUIOptionView : DragAndDropUIElement {

        [SerializeField] protected OptionType optionType;
        public OptionType OptionType => optionType;
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] bool showCharacterName = false;

        public MarkupPalette palette;

        [SerializeField] private UnityEvent onSelectedWhenBlocked;

        DialogueOption _option;

        //bool hasSubmittedOptionSelection = false;
        
        private void Awake() {
            EventTrigger trigger = gameObject.GetOrAddComponent<EventTrigger>();
            if (text == null) {
                text = GetComponent<TextMeshProUGUI>();
            }
            base.Configure(trigger);

            text.text = "";
            currentflag = FLAG_NONE;

            if (optionType == OptionType.Blocked) {

                var group = gameObject.GetComponent<CanvasGroup>();

                if (group == null) group = gameObject.AddComponent<CanvasGroup>();
                group.interactable = false;
                group.blocksRaycasts = false;

            }
            
            SetupUISounds();
        }

        private void SetupUISounds()
        {
            onPickup.AddListener((() => SoundManager.PlaySound(SoundManager.Sound.AnswerPickUp, SoundManager.Mixer.SFX)));
            onDrop.AddListener((() => SoundManager.PlaySound(SoundManager.Sound.AnswerDrop, SoundManager.Mixer.SFX)));
            onInvalidDrop.AddListener((() => SoundManager.PlaySound(SoundManager.Sound.AnswerBlocked, SoundManager.Mixer.SFX)));
            onDestruction.AddListener((() => SoundManager.PlaySound(SoundManager.Sound.AnswerBlocked, SoundManager.Mixer.SFX)));
        }

        public DialogueOption Option
        {
            get => _option;

            set
            {
                _option = value;

                //hasSubmittedOptionSelection = false;

                // When we're given an Option, use its text and update our
                // interactibility.
                Yarn.Markup.MarkupParseResult line;
                if (showCharacterName)
                {
                    line = value.Line.Text;
                }
                else
                {
                    line = value.Line.TextWithoutCharacterName;
                }

                if (palette != null)
                {
                    text.text = LineView.PaletteMarkedUpText(line, palette, false);
                }
                else
                {
                    text.text = line.Text;
                }
                
            }
        }

        protected override void OnClickDelegate(BaseEventData data) {
        
            base.OnClickDelegate(data);

            switch (optionType) {
                
                case OptionType.DestroyOnPickup:
                    StartDestructionSequence();
                    break;
                    
                case OptionType.Blocked:
                    onSelectedWhenBlocked.Invoke();
                    break;
                
            }

        }
        
    }
}
