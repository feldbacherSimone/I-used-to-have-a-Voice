using System;
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
            }

            _positionToggle = true;

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
        public void ShowClonedBox(bool show) {
        
            if (_mClonedBox == null) return;
            
            if (show) {
                _mClonedBox.SetActive(true);
                _mClonedBox.GetComponent<CanvasGroup>().alpha = 1;
            }
            else if (!_mFreezeClone) {
                Destroy(_mClonedBox);
                _mClonedBox = null;
            }
        }

        public CharacterBox CurrentCharacterBox() {
            return _comicBoxes[_mIndex];
        }

        public Vector3 GetContinueButtonPosition() {
            Vector3[] v = new Vector3[4];
            characterBoxPrefab.GetComponent<RectTransform>().GetWorldCorners(v);
            return v[3];
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
            
            var boxTransform = _comicBoxes[_mIndex].BoxRectTransform;

                var rect1 = boxTransform.rect;
                characterPositionEmpty.rect.Set(
                    rect1.x,
                    rect1.y,
                    rect1.width,
                    rect1.height
                );
                characterPositionEmpty.transform.SetPositionAndRotation(
                    boxTransform.position,
                    boxTransform.rotation
                );
                
                boxTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect1.width);
            
        }

        private void UpdatePrefabParameters(MarkupParseResult line) {
            
            var bg = CharacterBoxPrefab.GetComponent<Image>();
            var message = CharacterBoxPrefab.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            message.text = line.Text;
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

            var rectTransform = CharacterBoxPrefab.GetComponent<RectTransform>();
            Rect rect = CurrentCharacterBox().BoxRectTransform.rect;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
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

#endregion
        
        

    }
}
