using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Markup;
using Yarn.Unity.Example;

namespace _IUTHAV.Scripts.Dialogue {

    public class CharacterController : MonoBehaviour {

        private string _cName;
        [SerializeField] private RectTransform characterPositionEmpty;
        [SerializeField] private ContinueMode continueMode;
        [FormerlySerializedAs("continueTiming")] [SerializeField] private ContinueButtonTiming continueButtonTiming;

        private GameObject _mClonedBox;
        private bool _mFreezeClone;
        private MarkupParseResult _mLastLine;
        
        private bool _positionToggle;
        public string CName => _cName;
        public ContinueMode ContinueMode => continueMode;
        public ContinueButtonTiming ContinueButtonTiming => continueButtonTiming;

#region Unity Functions

        private void Awake() {

            if (characterPositionEmpty == null) {
                gameObject.GetComponent<RectTransform>();
            }

            _cName = characterPositionEmpty.gameObject.GetComponent<YarnCharacter>().characterName;
        }

#endregion

#region Public Functions

        public bool CheckPositionToggle() {
            bool buffer = _positionToggle;
            _positionToggle = false;
            return buffer;
        }
        
        public void SetCharEmptyPosition(RectTransform rectTransform) {
            
            characterPositionEmpty.transform.SetPositionAndRotation(
                rectTransform.position,
                rectTransform.rotation
            );
            _positionToggle = true;
        }
        
#endregion

#region Private Functions

        private IEnumerator FadeAndDestroyCanvasgroup(CanvasGroup group) {

            for (float i = 100; i > 0; i -= 10) {

                group.alpha = i/100f;
                
                yield return new WaitForSeconds(0.001f);
                
            }
            
            Destroy(group.gameObject);
        }

        private void LogWarning(string msg) {
            
            Debug.LogWarning("[CharacterBoxManager]["+_cName+"] " + msg);
        }

#endregion
        
        

    }
}
