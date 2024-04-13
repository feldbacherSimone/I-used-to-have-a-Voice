using System;
using UnityEngine;
using Yarn.Unity;

namespace _IUTHAV.Core_Programming.Dialogue {

    [Serializable]
    public class BoxSettings {
        public RectTransform boxTransform;
        public bool isRightAlignment;
    }
    /*TODO: Make this a Monobehaviour and populate it's RectTransforms automatically by getting it's children
     This way, minimal inspector dragging has to be done
     It also allows ComicBoxes to run coroutines, like fading themselves out
    */
    [Serializable]
    public class ComicBox {

        public string cName;
        public RectTransform currentCharacterTransform;
        public GameObject characterBoxPrefab;
        public BoxSettings[] boxSettings;
        
        private int _mIndex;
        private GameObject _mClonedBox;

        public void SetClonedBox(GameObject box) {
            _mClonedBox = box;
        }
        public void ShowClonedBox(bool show) {
        
            if (_mClonedBox == null) return;
            
            if (show) {
                _mClonedBox.SetActive(true);
                _mClonedBox.GetComponent<CanvasGroup>().alpha = 1;
            }
            else {
                GameObject.Destroy(_mClonedBox);
            }
        }
        
        public void NextPosition(bool silent = false) {
            
            if (!silent) _mIndex++;
            
            if (_mIndex < boxSettings.Length) {
                Rect boxRect = boxSettings[_mIndex].boxTransform.rect;
                currentCharacterTransform.rect.Set(
                    boxRect.x,
                    boxRect.y,
                    boxRect.width,
                    boxRect.height
                );
                currentCharacterTransform.transform.SetPositionAndRotation(
                    boxSettings[_mIndex].boxTransform.position,
                    boxSettings[_mIndex].boxTransform.rotation
                );
            }
            
        }

        public RectTransform CurrentTransform() {
            return boxSettings[_mIndex].boxTransform;
        }

        public bool CurrentAlignment() {
            return boxSettings[_mIndex].isRightAlignment;
        }
        
    }
}
