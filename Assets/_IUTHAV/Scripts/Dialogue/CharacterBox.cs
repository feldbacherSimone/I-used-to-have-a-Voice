using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _IUTHAV.Scripts.Dialogue {
    public class CharacterBox : MonoBehaviour {

        [Tooltip("Use the same name as given in the YarnCharacter Component! Will try to automatically assign the name using the __ as a seperator")]
        public string characterName;

        [SerializeField] protected TextMeshProUGUI text;
        [SerializeField] protected TextMeshProUGUI textSizeSetter;

        public TextMeshProUGUI Text => text;

        [SerializeField] private bool placeContinueButtonRight;
        
        public bool hideBoxOnBoxChange;

        [HideInInspector] public bool IsActive;
        
        protected RectTransform _boxRectTransform;
        
        public RectTransform BoxRectTransform => _boxRectTransform;
        
        [SerializeField] protected GameObject bubble;
        public bool PlaceContinueButtonRight => placeContinueButtonRight;

        public CanvasGroup BoxCanvasGroup;

        protected Vector3 initialScale;
        
        protected const float fadeTime = 0.3f;

        protected const float maxRandomWaitTime = 0.3f;

        private IEnumerator _mFadeBoxJob;

        private void Awake() {
        
            Configure();
        }

        private void Update() {

            textSizeSetter.text = text.text;

        }

        public void ToggleBubble(bool enable) {

            if (enable == IsActive) return;

            if (BoxCanvasGroup != null) {
                
                IsActive = enable;

                if (_mFadeBoxJob != null) {
                    StopCoroutine(_mFadeBoxJob);
                }
                _mFadeBoxJob = FadeBox(enable, BoxCanvasGroup);
                StartCoroutine(_mFadeBoxJob);
            }
            
        }
        
        public Vector3 GetContinueButtonPosition(string cName) {
            Vector3[] v = new Vector3[4];
            bubble.GetComponent<RectTransform>().GetWorldCorners(v);

            if (PlaceContinueButtonRight) {
                return v[0];
            }
            else {
                return v[3];
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

            if (bubble == null) {
                bubble = gameObject.transform.GetChild(0).transform.GetChild(0).gameObject;
            }

            if (BoxCanvasGroup == null && gameObject.transform.childCount > 0) {
                BoxCanvasGroup = gameObject.GetComponent<CanvasGroup>();
            }

            if (BoxCanvasGroup != null) {
            
                BoxCanvasGroup.alpha = 0;
                
                BoxCanvasGroup.blocksRaycasts = false;

                initialScale = transform.localScale;
                
                BoxCanvasGroup.transform.localScale = Vector3.zero;
                
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
