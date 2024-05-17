using System;
using System.Collections;
using _IUTHAV.Scripts.Core.Gamemode;
using _IUTHAV.Scripts.Core.Gamemode.CustomDataTypes;
using _IUTHAV.Scripts.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.CustomUI {

#region Helper Classes
    
    [Serializable]
    public class Bookmark {
        public float triggerPoint;
        public float endpoint;
        [Tooltip("Set to make the trigger finish a specified state. Will FREEZE that state upon finish!\n Choosing 'None' will set the 'SCX_ScrollTrigger' State to 'finished' until the next bookmark is active OR the user scrolls outside the triggerzone")]
        public StateType customState = StateType.None;
    }

#endregion
    
    public class ScrollBackGround : MonoBehaviour {

        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform bgRect;
        [SerializeField][Range(1, 15)] private float forceScrollSpeed = 4.0f;
        [Space(10)]
        [SerializeField] private int currentBmIndex;
        [SerializeField] private Bookmark[] bookmarks;

        [Space(10)] [SerializeField] private bool isDebug;

        

        private GameManager _mGameManager;
        private GameState _mCurrentTriggeredState;
        private float _mBgOffset;
        private Vector2 _mTargetPosition;

#region Unity Functions

        private void OnDrawGizmos() {

            Gizmos.matrix = canvasRect.localToWorldMatrix;
            var rect = bgRect.rect;
            var localPosition = bgRect.localPosition;
            
            Gizmos.color = Color.red;
            foreach (var f in bookmarks) {
                Gizmos.DrawLine(new Vector3(-rect.width/2, -f.endpoint + localPosition.y), new Vector3(rect.width/2, -f.endpoint + localPosition.y));
            }
            
            foreach (var f in bookmarks) {
                
                if (f.customState != StateType.None) {
                    Gizmos.color = Color.yellow;
                }
                else {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawLine(new Vector3(-rect.width/2, -f.triggerPoint + localPosition.y), new Vector3(-rect.width/2.5f, -f.triggerPoint + localPosition.y));
                Gizmos.DrawLine(new Vector3(rect.width/2.5f, -f.triggerPoint + localPosition.y), new Vector3(rect.width/2, -f.triggerPoint + localPosition.y));
            }
            Gizmos.color = Color.magenta;
            if (bookmarks.Length > 0) {
                float currentBmF = bookmarks[currentBmIndex].triggerPoint;
                Gizmos.DrawLine(new Vector3(-rect.width/3, -currentBmF + localPosition.y), new Vector3(rect.width/3, -currentBmF + localPosition.y));
            }
            

        }

        private void Start() {

            _mGameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

            if (scrollRect == null) {
                scrollRect = GetComponent<ScrollRect>();
            }

            _mBgOffset = bgRect.localPosition.y;
            
            SetTriggeredStateData();

            scrollRect.onValueChanged.AddListener(UpdateScrollStateData);
            
            StrechBackground();
        }

        private void OnDestroy() {
            scrollRect.onValueChanged.RemoveListener(UpdateScrollStateData);
        }

#endregion

#region Public Functions

        public void NextBookmark() {

            if (currentBmIndex < bookmarks.Length - 1) {
                _mCurrentTriggeredState.UnFinish();
                currentBmIndex++;
                StrechBackground();
                SetTriggeredStateData();
            }
            else {
                Log("No more bookmarks to iterate through!");
            }

        }

        public void ForceScroll(float yLocation) {

            scrollRect.enabled = false;
            _mTargetPosition = new Vector2(bgRect.anchoredPosition.x, yLocation);
            StartCoroutine(MoveToYLocation(false));
        }
        
        public void ForceScrollAndLock(float yLocation) {

            scrollRect.enabled = false;
            _mTargetPosition = new Vector2(bgRect.anchoredPosition.x, yLocation);
            StartCoroutine(MoveToYLocation(true));
        }

        public void ToggleManualScroll(bool enable) {

            scrollRect.enabled = enable;
        }

#endregion

#region Private Functions

        private void SetTriggeredStateData() {
            
            bool isScrollTrigger = (bookmarks[currentBmIndex].customState == StateType.None);
            
            if (isScrollTrigger) {
                
                StateType contextSensitiveState = Typeconverter.ChangePreAndSuffix(_mGameManager.GetCurrentSceneType(), StateType.SC1_ScrollTrigger);
                _mCurrentTriggeredState = _mGameManager.GetState(contextSensitiveState);
                _mCurrentTriggeredState.onStateCompleted.AddListener(NextBookmark);
            }
            else {
                _mCurrentTriggeredState = _mGameManager.GetState(bookmarks[currentBmIndex].customState);
                _mCurrentTriggeredState.isFreeze = true;
                Log("Next bookmark is a frozen Trigger of type [" + _mCurrentTriggeredState.StateType + "]");
            }
            
            _mCurrentTriggeredState.SetStateData(new FloatData(
                bgRect.localPosition.y,
                bookmarks[currentBmIndex].triggerPoint - _mBgOffset
                ));
        }

        private void UpdateScrollStateData(Vector2 pos) {
            
            _mCurrentTriggeredState.UpdateData(bgRect.localPosition.y);
        }

        private void StrechBackground() {
            
            bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bookmarks[currentBmIndex].endpoint);
        }

        private IEnumerator MoveToYLocation(bool lockOnFinish) {

            scrollRect.enabled = false;

            while (Vector2.Distance(bgRect.anchoredPosition, _mTargetPosition) > 0.1f) {
                
                var anchoredPosition = bgRect.anchoredPosition;
                
                Vector2 target = Vector2.MoveTowards(anchoredPosition, _mTargetPosition, forceScrollSpeed);
                
                anchoredPosition = new Vector3(anchoredPosition.x, target.y, anchoredPosition.x);
                bgRect.anchoredPosition = anchoredPosition;

                yield return null;
            }

            scrollRect.enabled = !lockOnFinish;
            yield return null;

        }

        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[ScrollBackground] " + msg);
        }

#endregion


    }
    
}
