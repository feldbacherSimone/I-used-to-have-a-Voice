using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _IUTHAV.Scripts.Dialogue {
    public class CharacterBox : MonoBehaviour {

        [Tooltip("Use the same name as given in the YarnCharacter Component! Will try to automatically assign the name using the __ as a seperator")]
        public string characterName;

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI textSizeSetter;

        public TextMeshProUGUI Text => text;

        [SerializeField] private bool placeContinueButtonRight;
        
        public bool hideBoxOnBoxChange;

        [HideInInspector] public bool IsActive;
        
        protected RectTransform _boxRectTransform;
        
        public RectTransform BoxRectTransform => _boxRectTransform;
        public bool PlaceContinueButtonRight => placeContinueButtonRight;

        [SerializeField] protected CanvasGroup _canvasGroup;

        protected Vector3 initialScale;
        
        protected const float fadeTime = 0.3f;

        protected const float maxRandomWaitTime = 0.3f;

        private void Awake() {
        
            Configure();
        }

        private void Update() {

            textSizeSetter.text = text.text;

        }

        public void ToggleBubble(bool enable) {

            if (enable == IsActive) return;

            if (_canvasGroup != null) {
                
                IsActive = enable;
                StartCoroutine(FadeBox(enable, _canvasGroup));
                
            }
            
        }

        protected void AutoAssignName() {

            var names = gameObject.name.Split("__");
            if (names != null && names.Length > 0) {
                characterName = names[0];
            }
        }

        protected void Configure() {
            
            RectTransform rectTransform = GetComponent<RectTransform>();

            _boxRectTransform = rectTransform;

            if (_canvasGroup == null && gameObject.transform.childCount > 0) {
                _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            }

            if (_canvasGroup != null) {
            
                _canvasGroup.alpha = 0;
                
                _canvasGroup.blocksRaycasts = false;

                initialScale = transform.localScale;
                
                _canvasGroup.transform.localScale = Vector3.zero;
                
            }
            
            if (characterName.Equals("")) AutoAssignName();
            
        }

        protected virtual IEnumerator FadeBox(bool enable, CanvasGroup group) {
        
            //Create an artificial waittime, so it feels more natural+

            yield return new WaitForSeconds(Random.Range(0, maxRandomWaitTime));

            float t = 0;

            while (t < fadeTime) {

                float x = (enable) ? Mathf.Lerp(0.1f, 1, t / fadeTime) : Mathf.Lerp(1, 0.1f, t / fadeTime);

                group.alpha = x;

                group.transform.localScale = Vector3.Scale(initialScale, new Vector3(x, x, x));

                t += Time.deltaTime;
                yield return null;
            }

            if (!enable) group.alpha = 0;

        }
    }
}
