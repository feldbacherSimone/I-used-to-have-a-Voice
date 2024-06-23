using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IUTHAV.Scripts.Core.Input;
using _IUTHAV.Scripts.Dialogue.Option;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Markup;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Dialogue {
    
    public class ComicBoxView : DialogueViewBase {
        
#region Yarn LineView Params
        
        private CanvasGroup _canvasGroup;
        
        [SerializeField]
        internal bool useFadeEffect = true;
        
        [SerializeField]
        [Min(0)]
        internal float fadeInTime = 0.25f;
        
        [SerializeField]
        [Min(0)]
        internal float fadeOutTime = 0.05f;
        
        [SerializeField]
        internal TextMeshProUGUI lineText = null;

        [SerializeField]
        internal bool useTypewriterEffect = false;
        
        [SerializeField]
        internal UnityEngine.Events.UnityEvent onCharacterTyped;

        [SerializeField] internal UnityEngine.Events.UnityEvent onPauseStarted;
        
        [SerializeField] internal UnityEngine.Events.UnityEvent onPauseEnded;
        
        [SerializeField]
        [Min(0)]
        internal float typewriterEffectSpeed = 0f;

        [SerializeField] internal bool ContinueOnClickAnywhere;
        [SerializeField]
        internal GameObject continueButton = null;

        /// <summary>
        /// The amount of time to wait after any line
        /// </summary>
        [SerializeField]
        [Min(0)]
        internal float holdTime = 1f;

        [SerializeField]
        internal MarkupPalette palette;

        /// <summary>
        /// The current <see cref="LocalizedLine"/> that this line view is
        /// displaying.
        /// </summary>
        LocalizedLine currentLine = null;

        /// <summary>
        /// A stop token that is used to interrupt the current animation.
        /// </summary>
        Effects.CoroutineInterruptToken currentStopToken = new Effects.CoroutineInterruptToken();
#endregion
        
        [Header("ComicBox Parameters")] 
        [SerializeField] private CharacterController[] characters;
        [SerializeField] private ConversationManager[] conversations;
        [Space(10)] [SerializeField] private bool isDebug;

        [SerializeField] private CharacterSounds characterSounds; 
        
        private Dictionary<string, CharacterController> _mCharControllers;
        private List<ConversationManager> _mConversations;
        private string _mCurrentChar;
        private int _mCurrentIndex;
        private string _mPreviousCharacter;
        private int _mPreviousConversationIndex = -1;
        private MarkupParseResult _mCurrentLine;
        private bool _mClickedContinue;
        private bool _mOverrideContinue;

        private DragUIOptionsManager _mCurrentOption;
        // The method we should call when an option has been selected.
        Action<int> OnOptionSelected;

        private LocalizedLine _lastLine;
        
        [SerializeField] bool showUnavailableOptions = false;
        
        private void Awake() {
            
            PopulateLists();
            _mCurrentChar = "None";
            SetCurrentCharacterDialogue(conversations[0].GetFirstCharacter());

            if (ContinueOnClickAnywhere) {
                continueButton = null;
                InputController.OnCustomClick += OnContinueClicked;
            }

            onCharacterTyped.AddListener(characterSounds.RequestVoiceSound);
        }

        private void Reset() {
            _canvasGroup = conversations[0].CurrentCharacterBox(conversations[0].GetFirstCharacter()).GetComponent<CanvasGroup>();
        }

        private void OnDisable() {
            if (ContinueOnClickAnywhere) {
                InputController.OnCustomClick -= OnContinueClicked;
            }
        }

#region Running Line
        /// <inheritdoc/>
        public override void DismissLine(Action onDismissalComplete)
        {
            currentLine = null;

            StartCoroutine(DismissLineInternal(onDismissalComplete));
        }

        private IEnumerator DismissLineInternal(Action onDismissalComplete)
        {
            // disabling interaction temporarily while dismissing the line
            // we don't want people to interrupt a dismissal
            var interactable = _canvasGroup.interactable;
            _canvasGroup.interactable = false;

            // If we're using a fade effect, run it, and wait for it to finish.
            if (useFadeEffect)
            {
                yield return StartCoroutine(Effects.FadeAlpha(_canvasGroup, 1, 0, fadeOutTime, currentStopToken));
                currentStopToken.Complete();
            }
            
            //_canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            // turning interaction back on, if it needs it
            _canvasGroup.interactable = interactable;
            
            if (onDismissalComplete != null)
            {
                onDismissalComplete();
            }
        }

        /// <inheritdoc/>
        public override void InterruptLine(LocalizedLine dialogueLine, Action onInterruptLineFinished)
        {
            currentLine = dialogueLine;

            // Cancel all coroutines that we're currently running. This will
            // stop the RunLineInternal coroutine, if it's running.
            StopAllCoroutines();
            
            // for now we are going to just immediately show everything
            // later we will make it fade in
            lineText.gameObject.SetActive(true);
            _canvasGroup.gameObject.SetActive(true);

            int length;
            
            lineText.text = dialogueLine.TextWithoutCharacterName.Text;
            length = dialogueLine.TextWithoutCharacterName.Text.Length;

            // Show the entire line's text immediately.
            lineText.maxVisibleCharacters = length;

            // Make the canvas group fully visible immediately, too.
            _canvasGroup.alpha = 1;

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            onInterruptLineFinished();
        }

        /// <inheritdoc/>
        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished) {

            _lastLine = dialogueLine;

            if (dialogueLine.CharacterName != null) {
                SetCurrentCharacterDialogue(dialogueLine.CharacterName);
            }
            SetCurrentLine(dialogueLine.TextWithoutCharacterName);

            //Show new box
            if (!_mConversations[_mCurrentIndex].CurrentCharacterBox(_mCurrentChar).IsActive) {
            
                _mConversations[_mCurrentIndex].ActivateBox(_mCurrentChar);
            }
            
            // Stop any coroutines currently running on this line view (for
            // example, any other RunLine that might be running)
            StopAllCoroutines();

            
            // Begin running the line as a coroutine.
            StartCoroutine(RunLineInternal(dialogueLine, onDialogueLineFinished));
        }

        private IEnumerator RunLineInternal(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {

            IEnumerator PresentLine() {

                lineText.gameObject.SetActive(true);
                _canvasGroup.gameObject.SetActive(true);

                // Hide the continue button until presentation is complete (if
                // we have one).
                if (continueButton != null)
                {
                    continueButton.SetActive(false);
                }

                Yarn.Markup.MarkupParseResult text = dialogueLine.TextWithoutCharacterName;

                // if we have a palette file need to add those colours into the text
                if (palette != null)
                {
                    lineText.text = LineView.PaletteMarkedUpText(text, palette);
                }
                else
                {
                    lineText.text = LineView.AddLineBreaks(text);
                }

                if (useTypewriterEffect)
                {
                    // If we're using the typewriter effect, hide all of the
                    // text before we begin any possible fade (so we don't fade
                    // in on visible text).
                    lineText.maxVisibleCharacters = 0;
                }
                else
                {
                    // Ensure that the max visible characters is effectively
                    // unlimited.
                    lineText.maxVisibleCharacters = int.MaxValue;
                }

                // If we're using the fade effect, start it, and wait for it to
                // finish.
                if (useFadeEffect)
                {
                    yield return StartCoroutine(Effects.FadeAlpha(_canvasGroup, 0, 1, fadeInTime, currentStopToken));
                    if (currentStopToken.WasInterrupted)
                    {
                        // The fade effect was interrupted. Stop this entire
                        // coroutine.
                        yield break;
                    }
                }

                // If we're using the typewriter effect, start it, and wait for
                // it to finish.
                if (useTypewriterEffect)
                {
                    var pauses = LineView.GetPauseDurationsInsideLine(text);

                    // setting the canvas all back to its defaults because if we didn't also fade we don't have anything visible
                    _canvasGroup.alpha = 1f;
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;

                    yield return StartCoroutine(Effects.PausableTypewriter(
                        lineText,
                        typewriterEffectSpeed,
                        () => onCharacterTyped.Invoke(),
                        () => onPauseStarted.Invoke(),
                        () => onPauseEnded.Invoke(),
                        pauses,
                        currentStopToken
                    ));

                    if (currentStopToken.WasInterrupted)
                    {
                        // The typewriter effect was interrupted. Stop this
                        // entire coroutine.
                        yield break;
                    }
                }
            }
            
            IEnumerator WaitForCustomContinue() {
                Log("[" + _mCurrentChar + "] Waiting for User Input PreLine");
                _mClickedContinue = false;
                while (!_mClickedContinue || !_mOverrideContinue) {
                    yield return null;
                }
                _mClickedContinue = false;
                _mOverrideContinue = false;
            }
            
            if (ShouldCharacterWaitForContinue(ContinueButtonTiming.PreLine)) {
                    
                if (continueButton != null) {
                    
                    continueButton.SetActive(true);
                    
                    continueButton.transform.SetPositionAndRotation(
                        _mConversations[_mCurrentIndex].CurrentCharacterBox(_mCurrentChar).BoxRectTransform.position,
                        _mConversations[_mCurrentIndex].CurrentCharacterBox(_mCurrentChar).BoxRectTransform.rotation
                    );
                    
                }
                yield return StartCoroutine(WaitForCustomContinue());
            }

            currentLine = dialogueLine;

            // Run any presentations as a single coroutine. If this is stopped,
            // which UserRequestedViewAdvancement can do, then we will stop all
            // of the animations at once.
            yield return StartCoroutine(PresentLine());

            currentStopToken.Complete();

            // All of our text should now be visible.
            lineText.maxVisibleCharacters = int.MaxValue;

            // Our view should at be at full opacity.
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            // Show the continue button, if we have one.
            if (continueButton != null && ShouldCharacterWaitForContinue(ContinueButtonTiming.PostLine))
            {
                continueButton.SetActive(true);
                
                continueButton.transform.SetPositionAndRotation(
                    _mConversations[_mCurrentIndex].GetContinueButtonPosition(_mCurrentChar),
                    _mConversations[_mCurrentIndex].CurrentCharacterBox(_mCurrentChar).transform.rotation
                );
            }

            // If we have a hold time, wait that amount of time, and then
            // continue.
            if (holdTime > 0)
            {
                yield return new WaitForSeconds(holdTime);
            }
            
            if (ShouldCharacterWaitForContinue(ContinueButtonTiming.PostLine))
            {
                Log("[" + _mCurrentChar + "] Waiting for User Input PostLine");
                // The line is now fully visible, and we've been asked to not
                // auto-advance to the next line. Stop here, and don't call the
                // completion handler - we'll wait for a call to
                // UserRequestedViewAdvancement, which will interrupt this
                // coroutine.
                yield break;
            }

            // Our presentation is complete; call the completion handler.
            onDialogueLineFinished();
        }
        
        /// <inheritdoc/>
        public override void UserRequestedViewAdvancement()
        {
            // We received a request to advance the view. If we're in the middle of
            // an animation, skip to the end of it. If we're not current in an
            // animation, interrupt the line so we can skip to the next one.

            // we have no line, so the user just mashed randomly
            if (currentLine == null)
            {
                return;
            }

            // we may want to change this later so the interrupted
            // animation coroutine is what actually interrupts
            // for now this is fine.
            // Is an animation running that we can stop?
            if (currentStopToken.CanInterrupt) 
            {
                // Stop the current animation, and skip to the end of whatever
                // started it.
                currentStopToken.Interrupt();
            }
            else
            {
                // No animation is now running. Signal that we want to
                // interrupt the line instead.
                requestInterrupt?.Invoke();
                
            }
        }
        
        /// <summary>
        /// Called when the <see cref="continueButton"/> is clicked.
        /// </summary>
        public void OnContinueClicked()
        {
            // When the Continue button is clicked, we'll do the same thing as
            // if we'd received a signal from any other part of the game (for
            // example, if a DialogueAdvanceInput had signalled us.)
            _mClickedContinue = true;
            UserRequestedViewAdvancement();
        }
        
        private void OnContinueClicked(InputAction.CallbackContext context) { OnContinueClicked(); }
        
        [YarnCommand("continue")]
        public void ForceContinue() {
            _mOverrideContinue = true;
            OnContinueClicked();
            var runner = GameObject.Find("DialogueRunner").GetComponent<DialogueRunner>();
            runner.Dialogue.Continue();
        }

        /// <inheritdoc />
        /// <remarks>
        /// If a line is still being shown dismisses it.
        /// </remarks>
        public override void DialogueComplete()
        {
            // do we still have a line lying around?
            if (currentLine != null)
            {
                currentLine = null;
                StopAllCoroutines();
                StartCoroutine(DismissLineInternal(null));
            }
        }

        /// <summary>
        /// Applies the <paramref name="palette"/> to the line based on it's markup.
        /// </summary>
        /// <remarks>
        /// This is static so that other dialogue views can reuse this code.
        /// While this is simplistic it is useful enough that multiple pieces might well want it.
        /// </remarks>
        /// <param name="line">The parsed marked up line with it's attributes.</param>
        /// <param name="palette">The palette mapping attributes to colours.</param>
        /// <param name="applyLineBreaks">If the [br /] marker is found in the line should this be replaced with a line break?</param>
        /// <returns>A TMP formatted string with the palette markup values injected within.</returns>
        public static string PaletteMarkedUpText(Yarn.Markup.MarkupParseResult line, MarkupPalette palette, bool applyLineBreaks = true)
        {
            string lineOfText = line.Text;
            line.Attributes.Sort((a, b) => (b.Position.CompareTo(a.Position)));
            foreach (var attribute in line.Attributes)
            {
                // we have a colour that matches the current marker
                Color markerColour;
                if (palette.ColorForMarker(attribute.Name, out markerColour))
                {
                    // we use the range on the marker to insert the TMP <color> tags
                    // not the best approach but will work ok for this use case
                    lineOfText = lineOfText.Insert(attribute.Position + attribute.Length, "</color>");
                    lineOfText = lineOfText.Insert(attribute.Position, $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(markerColour)}>");
                }

                if (applyLineBreaks && attribute.Name == "br")
                {
                    lineOfText = lineOfText.Insert(attribute.Position, "<br>");
                }
            }
            return lineOfText;
        }

        public static string AddLineBreaks(Yarn.Markup.MarkupParseResult line)
        {
            string lineOfText = line.Text;
            line.Attributes.Sort((a, b) => (b.Position.CompareTo(a.Position)));
            foreach (var attribute in line.Attributes.Where(a => a.Name == "br"))
            {
                // we then replace the marker with the tmp <br>
                lineOfText = lineOfText.Insert(attribute.Position, "<br>");
            }
            return lineOfText;
        }

        /// <summary>
        /// Creates a stack of typewriter pauses to use to temporarily halt the typewriter effect.
        /// </summary>
        /// <remarks>
        /// This is intended to be used in conjunction with the <see cref="Effects.PausableTypewriter"/> effect.
        /// The stack of tuples created are how the typewriter effect knows when, and for how long, to halt the effect.
        /// <para>
        /// The pause duration property is in milliseconds but all the effects code assumes seconds
        /// So here we will be dividing it by 1000 to make sure they interconnect correctly.
        /// </para>
        /// </remarks>
        /// <param name="line">The line from which we covet the pauses</param>
        /// <returns>A stack of positions and duration pause tuples from within the line</returns>
        public static Stack<(int position, float duration)> GetPauseDurationsInsideLine(Yarn.Markup.MarkupParseResult line)
        {
            var pausePositions = new Stack<(int, float)>();
            var label = "pause";
            
            // sorting all the attributes in reverse positional order
            // this is so we can build the stack up in the right positioning
            var attributes = line.Attributes;
            attributes.Sort((a, b) => (b.Position.CompareTo(a.Position)));
            foreach (var attribute in line.Attributes)
            {
                // if we aren't a pause skip it
                if (attribute.Name != label)
                {
                    continue;
                }

                // did they set a custom duration or not, as in did they do this:
                //     Alice: this is my line with a [pause = 1000 /]pause in the middle
                // or did they go:
                //     Alice: this is my line with a [pause /]pause in the middle
                if (attribute.Properties.TryGetValue(label, out Yarn.Markup.MarkupValue value))
                {
                    // depending on the property value we need to take a different path
                    // this is because they have made it an integer or a float which are roughly the same
                    // note to self: integer and float really ought to be convertible...
                    // but they also might have done something weird and we need to handle that
                    switch (value.Type)
                    {
                        case Yarn.Markup.MarkupValueType.Integer:
                            float duration = value.IntegerValue;
                            pausePositions.Push((attribute.Position, duration / 1000));
                            break;
                        case Yarn.Markup.MarkupValueType.Float:
                            pausePositions.Push((attribute.Position, value.FloatValue / 1000));
                            break;
                        default:
                            Debug.LogWarning($"Pause property is of type {value.Type}, which is not allowed. Defaulting to one second.");
                            pausePositions.Push((attribute.Position, 1));
                            break;
                    }
                }
                else
                {
                    // they haven't set a duration, so we will instead use the default of one second
                    pausePositions.Push((attribute.Position, 1));
                }
            }
            return pausePositions;
        }
#endregion
        
#region OptionFunctions

        public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected) {
            
            NextBox(_mCurrentChar);
            
            if (_lastLine.CharacterName != null) {
                SetCurrentCharacterDialogue(_lastLine.CharacterName);
            }
            SetCurrentLine(_lastLine.TextWithoutCharacterName);

            //Show new box
            

            // If we don't already have enough option views, create more
            var box = _mConversations[_mCurrentIndex].GetCurrentQuestionBox(_mCurrentChar);
            _mCurrentOption = box.Question;
            box.EnableQuestions();

            if (_mCurrentOption == null) {
                LogWarning("Not questionDropBoxes in " + _mConversations[_mCurrentIndex].CurrentCharacterBox(_mCurrentChar).gameObject.name + "have been found!");
                return;
            }
            
            while (dialogueOptions.Length > _mCurrentOption.GetOptionViewCount()) {
                
                var optionView = _mCurrentOption.CreateNewOptionView();
                optionView.gameObject.SetActive(false);
            }
            
            Log(_mCurrentOption.gameObject.name);
            // Set up all of the option views
            _mCurrentOption.SetupOptionViews(this.palette, dialogueOptions, showUnavailableOptions);
            _mCurrentOption.OnOptionSelected = OptionViewWasSelected;

            // Note the delegate to call when an option is selected
            OnOptionSelected = onOptionSelected;
        }
        
        private void OptionViewWasSelected(DialogueOption option) {
            _mConversations[_mCurrentIndex].ActivateBox(_mCurrentChar);
            OnOptionSelected(option.DialogueOptionID);
        }
        
#endregion

#region Public Functions

        [YarnCommand("nextBox")]
        public void NextBox(string cName) {

            CharacterBox box;
            
            if (cName.Equals("all")) {
                foreach (var cntrl in _mCharControllers.Values) {
                    if (!_mConversations[_mCurrentIndex].ContainsCharacter(cntrl.CName)) continue;
                    box = _mConversations[_mCurrentIndex].NextBox(cntrl.CName);
                
                    cntrl.SetCharEmptyPosition(box.BoxRectTransform);
                }
                return;
            }

            if (!CharacterExists(cName)) {
                LogWarning("Character [" + cName + "] doesn't exist!");
                return;
            }
            box = _mConversations[_mCurrentIndex].NextBox(cName);
            
            _mCharControllers[cName].SetCharEmptyPosition(box.BoxRectTransform);
            
            Log("NextBox for " + cName);
            
        }

        /// <summary>
        /// This is called by the DialogueRunner every time a node completes
        /// </summary>
        public void NextConversation() {

            if (_mCurrentIndex < _mConversations.Count -1) {
                _mPreviousConversationIndex = _mCurrentIndex;
                _mCurrentIndex++;
            }

            SetAllCharacterEmptyPositions();
            
            Log("Moving on to next Conversation " + _mConversations[_mCurrentIndex].gameObject.name);
        }
        
        public string GetCurrentCharacter() {
            return _mCurrentChar;
        }
        
#endregion

#region Private Functions

        private void SetCurrentLine(MarkupParseResult characterText) {
            _mCurrentLine = characterText;
        }

        private void SetCurrentCharacterDialogue(string newCharacter) {

            _mPreviousCharacter = _mCurrentChar;
            
            if (!CharacterExists(newCharacter)) {
                LogWarning("Character [" + newCharacter + "] doesn't exist!");
                return;
            }
            
            if (!newCharacter.Equals(_mCurrentChar) || _mPreviousConversationIndex != _mCurrentIndex) {

                var box = _mConversations[_mCurrentIndex].CurrentCharacterBox(newCharacter);
                _canvasGroup = box.GetComponent<CanvasGroup>();
                lineText = box.Text;

                characterSounds.SetCharacter(newCharacter);
                
                _mCurrentChar = newCharacter;
                //Log(box.gameObject.name + _mCurrentChar + " | " + _mCurrentIndex);
            }
            else {
                Log("Character was speaking already, no Box change required");
            }

        }

        private bool CharacterExists(string cName) {
            if (_mCharControllers.ContainsKey(cName)) {
                return true;
            }
            else {
                LogWarning("Character [" + cName + "] does not exist!");
                return false;
            }
        }

        private void PopulateLists() {
            
            var chars = GameObject.FindWithTag("CharacterContainer").GetComponentsInChildren<CharacterController>();
            if (chars.Length != conversations.Length) {
                
                LogWarning("Conversation List doesn't match Child count of ConversationContainer. Populating automatically");
                characters = chars;
            }

            _mCharControllers = new Dictionary<string, CharacterController>();
            foreach (var obj in characters) {
                _mCharControllers.Add(obj.CName, obj);
            }

            var convs = GameObject.FindWithTag("ConversationContainer").GetComponentsInChildren<ConversationManager>();
            if (convs.Length != conversations.Length) {
                
                LogWarning("Conversation List doesn't match Child count of ConversationContainer. Populating automatically");
                conversations = convs;
            }
            
            _mConversations = new List<ConversationManager>();
            for (int i = 0; i < conversations.Length; i++) {
                _mConversations.Add(conversations[i]);
            }
        }

        private void SetAllCharacterEmptyPositions() {
            
            foreach (var cntrl in _mCharControllers.Values) {

                if (_mConversations[_mCurrentIndex].ContainsCharacter(cntrl.CName)) {
                    var box = _mConversations[_mCurrentIndex].CurrentCharacterBox(cntrl.CName);
                
                    cntrl.SetCharEmptyPosition(box.BoxRectTransform);
                }
            }
            
        }

        private bool ShouldCharacterWaitForContinue(ContinueButtonTiming currentButtonTiming) {
            
            CharacterController charBox = _mCharControllers[_mCurrentChar];
            
            if (charBox.ContinueButtonTiming != currentButtonTiming) return false;

            switch (charBox.ContinueMode) {
                
                case ContinueMode.NeverWaitForContinue:
                    return false;
                
                case ContinueMode.AlwaysWaitForContinue:
                    return true;

                case ContinueMode.WaitUntilCharacterSwitch:
                    return !(_mPreviousCharacter.Equals(charBox.CName));
                    
                case ContinueMode.WaitUntilPanelSwitch:
                    return charBox.CheckPositionToggle();
                
                default:
                    return false;
            }
        }

        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[ComicBoxView] " + msg);
        }
        
        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[ComicBoxView] " + msg);
        }
#endregion
    }
        
}
