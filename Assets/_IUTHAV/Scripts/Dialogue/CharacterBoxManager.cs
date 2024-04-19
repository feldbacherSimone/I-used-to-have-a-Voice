using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Markup;
using Yarn.Unity.Example;

namespace _IUTHAV.Scripts.Dialogue {

    public class CharacterBoxManager : MonoBehaviour {

        private string _cName;
        [SerializeField] private Color gizmoColor;
        [SerializeField] private RectTransform characterPositionEmpty;
        [SerializeField] private GameObject characterBoxPrefab;
        [SerializeField] private ContinueMode continueMode;
        [SerializeField] private ContinueTiming continueTiming;
        
        private List<CharacterBox> _comicBoxes;
        private int _mIndex;
        private GameObject _mClonedBox;
        private bool _mFreezeClone;
        private MarkupParseResult _mLastLine;
        
        private bool _positionToggle;
        [HideInInspector] public bool dismissNextConversantClone;
        public string CName => _cName;
        public ContinueMode ContinueMode => continueMode;
        public ContinueTiming ContinueTiming => continueTiming;
        public GameObject CharacterBoxPrefab => characterBoxPrefab;

#region Unity Functions

        private void Awake() {

            Configure();

        }

        private void OnDrawGizmos() {
            Gizmos.color = gizmoColor;
            
            foreach (CharacterBox childCharacterBox in gameObject.transform.GetComponentsInChildren<CharacterBox>()) {
                RectTransform rectTransform = childCharacterBox.GetComponent<RectTransform>();
                Vector3[] v = new Vector3[4];
                rectTransform.GetWorldCorners(v);

                for (int i = 0; i < 3; i++) {
                    Gizmos.DrawLine(v[i], v[(i+1)%4]);
                }
                
                if (TryGetComponent(out TMP_Text previewText)) {
                    previewText.color = gizmoColor;
                }
            }
        }

#endregion

#region Public Functions

        public void NextPosition(bool silent = false) {
            
            if (!silent) _mIndex++;
            
            if (_mIndex < _comicBoxes.Count) {
                ApplyPrefabParameters();
                _positionToggle = true;
            }
            
        }

        public bool CheckPositionToggle() {
            bool buffer = _positionToggle;
            _positionToggle = false;
            return buffer;
        }
        
        public void CloneNewCharacterBox(MarkupParseResult line, bool freezeClone = false) {

            UpdatePrefabParameters(line);
            
            GameObject oldClone = InstantiateBox();
            
            ShowClonedBox(false);
            _mClonedBox = oldClone;
            _mFreezeClone = freezeClone;
            ShowClonedBox(true);
            
        }
        public void ShowClonedBox(bool show, bool fade = false) {
        
            if (_mClonedBox == null) return;
            
            if (show) {
                _mClonedBox.SetActive(true);
                _mClonedBox.GetComponent<CanvasGroup>().alpha = 1;
            }
            else if (!_mFreezeClone) {

                if (fade) {
                    StartCoroutine(FadeAndDestroyClonedBox());
                }
                else {
                    Destroy(_mClonedBox);
                    _mClonedBox = null;
                }
                
            }
        }

        public CharacterBox CurrentCharacterBox() {
            return _comicBoxes[_mIndex];
        }

        public Vector3 GetContinueButtonPosition() {
            Vector3[] v = new Vector3[4];
            characterBoxPrefab.GetComponent<RectTransform>().GetWorldCorners(v);

            if (CurrentCharacterBox().IsRightAlignment) {
                return v[0];
            }
            else {
                return v[3];
            }
        }
        
#endregion

#region Private Functions

        private void Configure() {
            
            _comicBoxes = new List<CharacterBox>();
            foreach (CharacterBox childCharacterBox in gameObject.transform.GetComponentsInChildren<CharacterBox>()) {
                _comicBoxes.Add(childCharacterBox);
            }

            _cName = characterPositionEmpty.gameObject.GetComponent<YarnCharacter>().characterName;
            
        }

        private void ApplyPrefabParameters() {
        
            var message = CharacterBoxPrefab.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            message.text = _mLastLine.Text;
            
            message.alignment = (CurrentCharacterBox().IsRightAlignment)
                ? TextAlignmentOptions.Right
                : TextAlignmentOptions.Left;
            
            var boxTransform = _comicBoxes[_mIndex].BoxRectTransform;
            characterPositionEmpty.transform.SetPositionAndRotation(
                boxTransform.position,
                boxTransform.rotation
            );
            var rectTransform = CharacterBoxPrefab.GetComponent<RectTransform>();
            Rect rect = CurrentCharacterBox().BoxRectTransform.rect;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
            
        }

        private void UpdatePrefabParameters(MarkupParseResult line) {
            
            var bg = CharacterBoxPrefab.GetComponent<Image>();
            _mLastLine = line;
            //message.color = currentTextColor;

            var layoutGroup = CharacterBoxPrefab.GetComponent<VerticalLayoutGroup>();
            if (_comicBoxes[_mIndex].IsRightAlignment) {
                layoutGroup.padding.left = 32;
                layoutGroup.padding.right = 0;
                bg.transform.SetAsLastSibling();
            }
            else {
                layoutGroup.padding.left = 0;
                layoutGroup.padding.right = 32;
                bg.transform.SetAsFirstSibling();
            }
            
        }
        
        private GameObject InstantiateBox() {
        
            GameObject oldClone = Instantiate( 
                    CharacterBoxPrefab,
                    CurrentCharacterBox().BoxRectTransform.position,
                    CurrentCharacterBox().BoxRectTransform.rotation,
                    CurrentCharacterBox().gameObject.transform
            );
            return oldClone;
        }

        private IEnumerator FadeAndDestroyClonedBox() {

            CanvasGroup group = _mClonedBox.GetComponentInChildren<CanvasGroup>();

            for (float i = 100; i > 0; i -= 10) {

                group.alpha = i/100f;
                
                yield return new WaitForSeconds(0.001f);
                
            }
            
            Destroy(_mClonedBox);
            _mClonedBox = null;
        }

#endregion
        
        

    }
}
