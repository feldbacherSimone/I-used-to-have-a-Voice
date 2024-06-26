using System.Collections;
using _IUTHAV.Scripts.Dialogue.Option;
using UnityEngine;

namespace _IUTHAV.Scripts.Dialogue {
    public class QuestionBox : CharacterBox {


        [SerializeField] private DragUIOptionsManager question;
        
        [SerializeField] private CanvasGroup questionCanvasGroup;

        public DragUIOptionsManager Question => question;

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

                initialScale = bubble.transform.localScale;

                bubble.transform.localScale = Vector3.zero;

                question.gameObject.SetActive(false);

            }
            
            if (characterName.Equals("")) AutoAssignName();
        }

        public void EnableQuestions(bool enable = true) {
            
            if (enable) Question.gameObject.SetActive(true);
            StartCoroutine(FadeBox(enable, questionCanvasGroup));
        }
        
        protected override IEnumerator FadeBox(bool enable, CanvasGroup group) {

            group.interactable = enable;
            group.blocksRaycasts = enable;
            
            yield return new WaitForSeconds(Random.Range(0, maxRandomWaitTime));
            
            float t = 0;
            
            while (t < fadeTime) {
                
                float x = (enable) ? Mathf.Lerp(0.1f, 1, t / fadeTime) : Mathf.Lerp(1, 0.1f, t / fadeTime);
                
                group.alpha = x;
                
                if (group == BoxCanvasGroup) bubble.transform.localScale = Vector3.Scale(initialScale, new Vector3(x, x, x));
                
                t += Time.deltaTime;
                yield return null;
            }

            if (!enable) group.alpha = 0;
            
            if (group == questionCanvasGroup && !enable) Question.gameObject.SetActive(false);

        }
        
    }
}