using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.Dialogue {
    public class PhonechatBox : CharacterBox {

        private RectTransform _rectTransform;
        private RectTransform _phoneGroup;

        [SerializeField] [Range(0, 2)] private float spacingFactor = 1.5f;

        private bool ForceUpdatePhoneBox;
        
        private void Awake() {
        
            Configure();
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _phoneGroup = gameObject.transform.parent.gameObject.GetComponent<RectTransform>();
        }

        private void Update() {

            textSizeSetter.text = text.text;

            float height = textSizeSetter.rectTransform.rect.height;
            
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
            height * transform.localScale.y
            * spacingFactor);

            if (ForceUpdatePhoneBox) {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_phoneGroup);
            }
            
        }

        public void ForceUpdatePhonebox(bool enable) {
            ForceUpdatePhoneBox = enable;
        }

        protected override IEnumerator FadeBox(bool enable, CanvasGroup group) {
        
            //Create an artificial waittime, so it feels more natural
            
            yield return new WaitForSeconds(Random.Range(0, maxRandomWaitTime));

            float t = 0;

            while (t < fadeTime) {

                float x = (enable) ? Mathf.Lerp(0.1f, 1, t / fadeTime) : Mathf.Lerp(1, 0.1f, t / fadeTime);
                
                group.alpha = x;

                group.transform.localScale = Vector3.Scale(initialScale, new Vector3(x, x, x));
                
                t += Time.deltaTime;
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(_phoneGroup);
                
                yield return null;
            }

            if (!enable) group.alpha = 0;

        }
        
    }
}