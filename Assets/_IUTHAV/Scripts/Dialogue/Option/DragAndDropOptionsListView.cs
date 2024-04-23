using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Dialogue.Option {
    public class DragAndDropOptionsListView : DialogueViewBase {
        
        [SerializeField] CanvasGroup canvasGroup;

        [SerializeField] MarkupPalette palette;

        [SerializeField] float fadeTime = 0.1f;

        [SerializeField] bool showUnavailableOptions = false;

        [Header("Last Line Components")]
        [SerializeField] TextMeshProUGUI lastLineText;
        [SerializeField] GameObject lastLineContainer;

        [SerializeField] TextMeshProUGUI lastLineCharacterNameText;
        [SerializeField] GameObject lastLineCharacterNameContainer;

        [Header("DragBoxView Parameters")] [SerializeField]
        private DragUIOptionsManager[] dropBoxes;

        [SerializeField] private bool isDebug;

        // A cached pool of OptionView objects so that we can reuse them
        List<DragUIOptionsManager> _mDropBoxes;
        private int _mIndex = 0;

        // The method we should call when an option has been selected.
        Action<int> OnOptionSelected;

        // The line we saw most recently.
        LocalizedLine lastSeenLine;

#region Unity Functions
        public void Awake() {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            _mDropBoxes = new List<DragUIOptionsManager>();
            foreach (DragUIOptionsManager view in dropBoxes) {
                _mDropBoxes.Add(view);
            }
        }

        public void Reset() {
            canvasGroup = GetComponentInParent<CanvasGroup>();
        }
        
        public void OnEnable() {
            Relayout();
        }
#endregion

#region Public Functions

        [YarnCommand("nextOption")]
        public void NextOption() {

            if (_mIndex < _mDropBoxes.Count) {
                _mIndex++;
            }
            else {
                LogWarning("Out of OptionManagers! Remaining at last one");
            }

        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished) {
            // Don't do anything with this line except note it and
            // immediately indicate that we're finished with it. RunOptions
            // will use it to display the text of the previous line.
            lastSeenLine = dialogueLine;
            onDialogueLineFinished();
        }
        public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            Log(_mDropBoxes[_mIndex].gameObject.name);
            // If we don't already have enough option views, create more
            while (dialogueOptions.Length > _mDropBoxes[_mIndex].GetOptionViewCount()) {
                var optionView = _mDropBoxes[_mIndex].CreateNewOptionView();
                optionView.gameObject.SetActive(false);
            }
            
            // Set up all of the option views
            _mDropBoxes[_mIndex].SetupOptionViews(this.palette, dialogueOptions, showUnavailableOptions);
            _mDropBoxes[_mIndex].OnOptionSelected = OptionViewWasSelected;
            _mDropBoxes[_mIndex].gameObject.SetActive(true);

            ConfigureLastLine();

            // Note the delegate to call when an option is selected
            OnOptionSelected = onOptionSelected;

            // sometimes (not always) the TMP layout in conjunction with the
            // content size fitters doesn't update the rect transform
            // until the next frame, and you get a weird pop as it resizes
            // just forcing this to happen now instead of then
            Relayout();

            // Fade it all in
            StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeTime));
        }
        
        /// <inheritdoc />
        /// <remarks>
        /// If options are still shown dismisses them.
        /// </remarks>
        public override void DialogueComplete()
        {   
            // do we still have any options being shown?
            if (canvasGroup.alpha > 0)
            {
                StopAllCoroutines();
                lastSeenLine = null;
                OnOptionSelected = null;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                StartCoroutine(FadeAndDisableOptionViews(canvasGroup, canvasGroup.alpha, 0, fadeTime));
            }
        }

#endregion

#region Private Functions

        private void OptionViewWasSelected(DialogueOption option)
            {
                StartCoroutine(OptionViewWasSelectedInternal(option));

                IEnumerator OptionViewWasSelectedInternal(DialogueOption selectedOption)
                {
                    yield return StartCoroutine(FadeAndDisableOptionViews(canvasGroup, 1, 0, fadeTime));
                    OnOptionSelected(selectedOption.DialogueOptionID);
                }
            }

        /// <summary>
        /// Fades canvas and then disables all option views.
        /// </summary>
        private IEnumerator FadeAndDisableOptionViews(CanvasGroup canvasGroup, float from, float to, float fadeTime)
        {
            yield return Effects.FadeAlpha(canvasGroup, from, to, fadeTime);

            // Hide all existing option views
            _mDropBoxes[_mIndex].gameObject.SetActive(false);
        }

        private void Relayout()
        {
            // Force re-layout
            var layouts = GetComponentsInChildren<UnityEngine.UI.LayoutGroup>();

            // Perform the first pass of re-layout. This will update the inner horizontal group's sizing, based on the text size.
            foreach (var layout in layouts)
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
            }
            
            // Perform the second pass of re-layout. This will update the outer vertical group's positioning of the individual elements.
            foreach (var layout in layouts)
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
            }
        }

        private void ConfigureLastLine() {
            // Update the last line, if one is configured
            if (lastLineContainer != null)
            {
                if (lastSeenLine != null)
                {
                    // if we have a last line character name container
                    // and the last line has a character then we show the nameplate
                    // otherwise we turn off the nameplate
                    var line = lastSeenLine.Text;
                    if (lastLineCharacterNameContainer != null)
                    {
                        if (string.IsNullOrWhiteSpace(lastSeenLine.CharacterName))
                        {
                            lastLineCharacterNameContainer.SetActive(false);
                        }
                        else
                        {
                            line = lastSeenLine.TextWithoutCharacterName;
                            lastLineCharacterNameContainer.SetActive(true);
                            lastLineCharacterNameText.text = lastSeenLine.CharacterName;
                        }
                    }

                    if (palette != null)
                    {
                        lastLineText.text = LineView.PaletteMarkedUpText(line, palette);
                    }
                    else
                    {
                        lastLineText.text = line.Text;
                    }

                    lastLineContainer.SetActive(true);
                }
                else
                {
                    lastLineContainer.SetActive(false);
                }
            }
        }
        
        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[DragAndDropOptionsListView] " + msg);
        }
        
        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[DragAndDropOptionsListView] " + msg);
        }
#endregion
    }
}
