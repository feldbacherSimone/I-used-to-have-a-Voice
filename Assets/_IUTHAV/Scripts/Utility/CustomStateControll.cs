using System;
using _IUTHAV.Scripts.Core.Gamemode;
using _IUTHAV.Scripts.CustomUI;
using _IUTHAV.Scripts.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Utility
{
    public class CustomStateControll : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        
        [SerializeField] private ScrollBackGround _scrollBackGround;
        
        [SerializeField] private DialogueRunner dialogueRunner;

        [SerializeField] private ComicBoxView comicBoxView;
        
        [SerializeField] private TextMeshProUGUI timeScaleText;
        
        [SerializeField] private TextMeshProUGUI conversationText;
        [SerializeField] private TextMeshProUGUI conversationTextSetter;
        [SerializeField] private Image conversationColorDisplay;
        
        private int bookmarkCount;

        private float _currentTimeFactor = 1;
        private void Start()
        {
            bookmarkCount = _scrollBackGround.bookmarkCount;
            _currentTimeFactor = Time.timeScale;
            
            timeScaleText.text = "x " + _currentTimeFactor;
            timeScaleText.gameObject.SetActive(false);

            comicBoxView.OnLineRun += UpdateLineText;
            
        }

        private void OnDisable() {
            comicBoxView.OnLineRun -= UpdateLineText;

            Time.timeScale = 1.0f;
        }

        public void ClearAllBookmarks()
        {
            for (int i = 0; i < bookmarkCount; i++)
            {
                _scrollBackGround.NextBookmark();
            }
            _scrollBackGround.ToggleManualScroll(true);
        }

        public void NextBookmark()
        {
            if (bookmarkCount > 0)
            {
                _scrollBackGround.NextBookmark();
            }
            _scrollBackGround.ToggleManualScroll(true);
        }

        public void SkipConversation() {
            
            gameManager.FinishState(dialogueRunner.CurrentNodeName);
            dialogueRunner.Stop();
            comicBoxView.NextConversation();

        }

        public void FastForward() {
            
            if (_currentTimeFactor < 8.1f) {
                _currentTimeFactor *= 2;
                Time.timeScale *= 2;
                
                
                timeScaleText.gameObject.SetActive(true);
                timeScaleText.text = "x " + _currentTimeFactor;
            }

        }

        public void ResetTimeScale() {
            
            Time.timeScale = 1.0f;
            _currentTimeFactor = 1.0f;
            
            timeScaleText.text = "x " + _currentTimeFactor;
            timeScaleText.gameObject.SetActive(false);
        }

        public void PauseTime() {

            _currentTimeFactor = 0.001f;
            Time.timeScale *= 0.001f;
            
        }

        private void UpdateLineText(ConversationManager conv) {
            conversationText.text = "Current Box: \n" + conv.CurrentCharacterBox(comicBoxView.GetCurrentCharacter())?.gameObject.name;
            
            if (conversationColorDisplay != null) {
                conversationColorDisplay.color = conv.GizmoColor;
            }
            
            conversationTextSetter.text = "Current Box: \n" + conv.CurrentCharacterBox(comicBoxView.GetCurrentCharacter())?.gameObject.name;
            
        }
    }
}