using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _IUTHAV.Scripts.Dialogue {
    public class CharacterBox : MonoBehaviour {
    
        private RectTransform _boxRectTransform;
        [SerializeField] private bool isRightAlignment;
        
        public RectTransform BoxRectTransform => _boxRectTransform;
        public bool IsRightAlignment => isRightAlignment;

        public CanvasGroup BubbleGroup;

        private Vector3 initialScale;

        private const float fadeTime = 0.3f;

        private const float maxRandomWaitTime = 0.3f;

        private void Awake() {
        
            RectTransform rectTransform = GetComponent<RectTransform>();

            _boxRectTransform = rectTransform;

            if (BubbleGroup == null && gameObject.transform.childCount > 0) {
                BubbleGroup = gameObject.transform.GetChild(0)?.gameObject.GetComponent<CanvasGroup>();
            }

            if (BubbleGroup != null) {
            
                BubbleGroup.alpha = 0;
                
                initialScale = BubbleGroup.transform.localScale;
                
                BubbleGroup.transform.localScale = Vector3.zero;
            }
        }

        public void ToggleBubble(bool enable) {

            if (BubbleGroup != null) {

                StartCoroutine(FadeBubble(enable));

            }
            
        }

        private IEnumerator FadeBubble(bool enable) {
        
            //Create an artificial waittime, so it feels more natural+

            yield return new WaitForSeconds(Random.Range(0, maxRandomWaitTime));

            float t = 0;

            while (t < fadeTime) {

                float x = (enable) ? Mathf.Lerp(0, 1, t / fadeTime) : Mathf.Lerp(1, 0, t / fadeTime);

                BubbleGroup.alpha = x;

                BubbleGroup.transform.localScale = Vector3.Scale(initialScale, new Vector3(x, x, x));

                t += Time.deltaTime;
                yield return null;
            }

        }
    }
}
