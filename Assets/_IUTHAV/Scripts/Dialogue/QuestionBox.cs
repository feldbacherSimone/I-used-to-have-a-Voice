﻿using System.Collections;
using _IUTHAV.Scripts.Dialogue.Option;
using UnityEngine;

namespace _IUTHAV.Scripts.Dialogue {
    public class QuestionBox : CharacterBox {

        [SerializeField] private DragUIOptionsManager question;
        
        [SerializeField] private CanvasGroup questionCanvasGroup;
        public DragUIOptionsManager Question => question;

        [SerializeField] private bool hideOptionsOnSubmit = true;

        private IEnumerator _mFadeQuestionJob;

        private void Awake() {
            RectTransform rectTransform = GetComponent<RectTransform>();

            _boxRectTransform = rectTransform;

            if (BoxCanvasGroup == null) {
                BoxCanvasGroup = gameObject.transform.GetChild(1).gameObject.GetComponent<CanvasGroup>();
            }

            if (BoxCanvasGroup != null) {
            
                BoxCanvasGroup.alpha = 0;
                questionCanvasGroup.alpha = 0;
                
                BoxCanvasGroup.blocksRaycasts = false;
                BoxCanvasGroup.interactable = false;
                questionCanvasGroup.blocksRaycasts = false;
                questionCanvasGroup.interactable = false;

                initialScale = Vector3.one;

                BoxCanvasGroup.transform.localScale = Vector3.zero;

                question.gameObject.SetActive(false);

            }
            
            if (characterName.Equals("")) AutoAssignName();
        }

        public void EnableQuestions(bool enable = true) {
            
            if (enable) Question.gameObject.SetActive(true);

            if (_mFadeQuestionJob != null) {
                StopCoroutine(_mFadeQuestionJob);
            }

            _mFadeQuestionJob = FadeBox(enable, questionCanvasGroup);
            StartCoroutine(_mFadeQuestionJob);
        }
        
        protected override IEnumerator FadeBox(bool enable, CanvasGroup group) {

            group.interactable = enable;
            group.blocksRaycasts = enable;
            
            yield return new WaitForSeconds(Random.Range(0, maxRandomWaitTime));
            
            float t = 0;
            
            while (t < fadeTime) {
                
                float x = (enable) ? Mathf.Lerp(0.01f, 1, t / fadeTime) : Mathf.Lerp(1, 0.01f, t / fadeTime);


                if (group == questionCanvasGroup) {

                    if (enable) {
                        group.alpha = x;
                    }
                    
                    if (!enable && hideOptionsOnSubmit) {
                        group.alpha = x;
                    }
                    
                }
                

                if (group == BoxCanvasGroup) {
                    BoxCanvasGroup.transform.localScale = Vector3.Scale(initialScale, new Vector3(x, x, x));
                    group.alpha = x;
                }

                t += Time.deltaTime;
                yield return null;
            }

            //if (!enable) group.alpha = 0;
            
            if (group == questionCanvasGroup && !enable && hideOptionsOnSubmit) Question.gameObject.SetActive(false);

        }
        
    }
}