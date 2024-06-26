using System;
using System.Collections.Generic;
using _IUTHAV.Scripts.CustomUI;
using UnityEngine;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Dialogue.Option {
    public class DragUIOptionsManager : DropBox {
        
        [SerializeField] DragableUIOptionView optionViewPrefab;
        
        public Action<DialogueOption> OnOptionSelected;
        DialogueOption _mOption;
        bool hasSubmittedOptionSelection = false;

        private List<DragableUIOptionView> _mOptionViews;

        private void Awake() {
        
            base.Configure();

            _mOptionViews = new List<DragableUIOptionView>();

            for (int i = 0; i < transform.childCount; i++) {
                
                if (transform.GetChild(i).gameObject.TryGetComponent(out DragableUIOptionView view)) {
                    _mOptionViews.Add(view);
                }
            }

        }

        public DragableUIOptionView CreateNewOptionView() {
        
            var optionView = Instantiate(optionViewPrefab);
            optionView.transform.SetParent(transform, false);
            optionView.transform.SetAsLastSibling();
            
            _mOptionViews.Add(optionView);
            LogWarning("Not enough Options in children found - making new ones");

            return optionView;
        }

        public int GetOptionViewCount() {
            Log("Current Option views: " + _mOptionViews.Count);
            return _mOptionViews.Count;
        }

        public void SetupOptionViews(MarkupPalette palette, DialogueOption[] dialogueOptions, bool showUnavailableOptions) {
            
            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                var optionView = _mOptionViews[i];
                var option = dialogueOptions[i];

                if (option.IsAvailable == false && showUnavailableOptions == false)
                {
                    // Don't show this option.
                    continue;
                }

                optionView.gameObject.SetActive(true);

                optionView.palette = palette;
                optionView.Option = option;
            }
            
        }

        public void InvokeOptionSelected()
        {
            // We only want to invoke this once, because it's an error to
            // submit an option when the Dialogue Runner isn't expecting it. To
            // prevent this, we'll only invoke this if the flag hasn't been cleared already.
            if (hasSubmittedOptionSelection == false)
            {
                OnOptionSelected?.Invoke(_mOption);
                hasSubmittedOptionSelection = true;
            }
        }

        protected override void OnDropElementDropped(DragAndDropUIElement dropElement) {
            Debug.Log(gameObject.name + " received " + dropElement.gameObject.name);
            if (dropElement.gameObject.TryGetComponent(out DragableUIOptionView optionView)) {

                switch (optionView.OptionType) {
                
                    case OptionType.Valid:
                    
                        _mOption = optionView.Option;
                        IsFull = true;
                
                        dropElement.SnapToTarget(transform.position, () => {
                            dropElement.currentflag = DragUIElement.FLAG_LOCK;
                            this.InvokeOptionSelected();
                            dropElement.StartValidDropPointSequence();
                        });
                        
                        break;
                    
                    case OptionType.DestroyOnDrop:
                    
                        dropElement.SnapToTarget(transform.position, () => {
                            dropElement.currentflag = DragUIElement.FLAG_LOCK;
                            dropElement.StartDestructionSequence();
                        });
                        Log("Chose Invalid Option");
                        break;
                }

            }
            else {
                dropElement.StartInvalidDropPointSequence();
                Log("Invalid DragUIelement [" + dropElement.gameObject.name + "]");
            }
            
        }
        
        private new void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[DragUIElementOptionsManager] " + msg);
        }
        
        private new void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[DragUIElementOptionsManager] " + msg);
        }

    }
}
