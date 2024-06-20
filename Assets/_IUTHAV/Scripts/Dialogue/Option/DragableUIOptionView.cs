using System;
using _IUTHAV.Scripts.CustomUI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Dialogue.Option {
    public class DragableUIOptionView : DragAndDropUIElement {

        [SerializeField] protected OptionType optionType;
        public OptionType OptionType => optionType;
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] bool showCharacterName = false;

        public MarkupPalette palette;

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
            if (optionType == OptionType.DestroyOnPickup) {
                StartDestructionSequence();
                
            }
            
        }
        
    }
}
