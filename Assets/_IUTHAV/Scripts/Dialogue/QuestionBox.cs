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

            if (_canvasGroup == null && gameObject.transform.childCount > 0) {
                _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            }

            if (_canvasGroup != null) {
            
                _canvasGroup.alpha = 0;
                questionCanvasGroup.alpha = 0;
                
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
                questionCanvasGroup.blocksRaycasts = false;
                questionCanvasGroup.interactable = false;

                initialScale = bubble.transform.localScale;

                bubble.transform.localScale = Vector3.zero;

            }
            
            if (characterName.Equals("")) AutoAssignName();
        }

        public void EnableQuestions() {
            
            StartCoroutine(FadeBox(true, questionCanvasGroup));
        }
        
        protected override IEnumerator FadeBox(bool enable, CanvasGroup group) {
        
            //Create an artificial waittime, so it feels more natural+
            group.interactable = enable;
            group.blocksRaycasts = enable;

            yield return new WaitForSeconds(Random.Range(0, maxRandomWaitTime));

            float t = 0;

            while (t < fadeTime) {

                float x = (enable) ? Mathf.Lerp(0.1f, 1, t / fadeTime) : Mathf.Lerp(1, 0.1f, t / fadeTime);

                group.alpha = x;
                
                if (group == _canvasGroup) bubble.transform.localScale = Vector3.Scale(initialScale, new Vector3(x, x, x));
                
                t += Time.deltaTime;
                yield return null;
            }

            if (!enable) group.alpha = 0;

        }
        
    }
}