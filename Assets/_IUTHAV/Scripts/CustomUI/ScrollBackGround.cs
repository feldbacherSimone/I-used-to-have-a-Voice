using System;
using System.Collections;
using _IUTHAV.Scripts.Core.Gamemode;
using _IUTHAV.Scripts.Core.Gamemode.CustomDataTypes;
using _IUTHAV.Scripts.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Yarn.Unity;

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

        [SerializeField] private Animator scrollIndicator;

        [Space(10)] [SerializeField] private bool isDebug;


        public int bookmarkCount; 

        private GameManager _mGameManager;
        private GameState _mCurrentTriggeredState;
        private float _mBgOffset;
        private Vector2 _mTargetPosition;

#region Unity Functions

        private void OnDrawGizmos() {

            //Gizmos.matrix = canvasRect.localToWorldMatrix;
            var rect = bgRect.rect;
            var localY = bgRect.anchoredPosition.y + canvasRect.rect.height;
            
            Gizmos.color = Color.red;
            foreach (var f in bookmarks) {
                Gizmos.DrawLine(new Vector3(0, -f.endpoint + localY), new Vector3(rect.width, -f.endpoint + localY));
            }
            
            foreach (var f in bookmarks) {
                
                if (f.customState != StateType.None) {
                    Gizmos.color = Color.yellow;
                }
                else {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawLine(new Vector3(0, -f.triggerPoint + localY), new Vector3(200, -f.triggerPoint + localY));
                Gizmos.DrawLine(new Vector3(rect.width-200, -f.triggerPoint + localY), new Vector3(rect.width, -f.triggerPoint + localY));
            }
            Gizmos.color = Color.magenta;
            if (bookmarks.Length > 0) {
                float currentBmF = bookmarks[currentBmIndex].triggerPoint;
                Gizmos.DrawLine(new Vector3(300, -currentBmF + localY), new Vector3(rect.width - 300, -currentBmF + localY));
            }
            

        }

        private void Awake()
        {
            bookmarkCount = bookmarks.Length;
        }

        private void Start() {

            _mGameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

            if (scrollRect == null) {
                scrollRect = GetComponent<ScrollRect>();
            }

            _mBgOffset = bgRect.anchoredPosition.y;
            Log("Starting with Offset " + _mBgOffset);
            
            SetTriggeredStateData();

            scrollRect.onValueChanged.AddListener(UpdateScrollStateData);
            
            StrechBackground();
            
        }

        private void OnDestroy() {
            scrollRect.onValueChanged.RemoveListener(UpdateScrollStateData);
        }

#endregion

#region Public Functions

        [YarnCommand("nextBookmark")]
        public void NextBookmark() {

            if (currentBmIndex < bookmarks.Length - 1) {
                _mCurrentTriggeredState?.UnFinish();
                currentBmIndex++;
                StrechBackground();
                SetTriggeredStateData();
                
                scrollIndicator.Play("On");
            }
            else {
                Log("No more bookmarks to iterate through!");
            }

        }
        [YarnCommand("forceScrollToEndpoint")]
        public void ForceScrollToEndpoint() {

            scrollRect.enabled = false;
            _mTargetPosition = new Vector2(bgRect.anchoredPosition.x, bookmarks[currentBmIndex].endpoint-canvasRect.rect.height);
            StartCoroutine(MoveToYLocation(false));
            
            scrollIndicator.Play("Off");
            
        }
        
        [YarnCommand("forceScrollAndLock")]
        public void ForceScrollAndLock() {

            scrollRect.enabled = false;
            _mTargetPosition = new Vector2(bgRect.anchoredPosition.x, bookmarks[currentBmIndex].endpoint-canvasRect.rect.height);
            StartCoroutine(MoveToYLocation(true));
            
            scrollIndicator.Play("Off");
            
        }
        [YarnCommand("unlock")]
        public void ToggleManualScroll(bool enable) {

            scrollRect.enabled = enable;
        }

        [YarnCommand("showIndicator")]
        public void ShowIndicator() {
            scrollIndicator.Play("On");
        }

#endregion

#region Private Functions

        private void SetTriggeredStateData() {

            if (bookmarks[currentBmIndex].customState == StateType.None) {
                return;
            }
            
            _mCurrentTriggeredState = _mGameManager.GetState(bookmarks[currentBmIndex].customState);
            _mCurrentTriggeredState.isFreeze = true;
            Log("Next bookmark is a frozen Trigger of type [" + _mCurrentTriggeredState.StateType + "]");
            
            _mCurrentTriggeredState.SetStateData(new FloatData(
                bgRect.anchoredPosition.y,
                bookmarks[currentBmIndex].triggerPoint - _mBgOffset - canvasRect.rect.height
            ));
        }

        private void UpdateScrollStateData(Vector2 pos) {
            
            _mCurrentTriggeredState?.UpdateData(bgRect.anchoredPosition.y);
            
        }

        private void StrechBackground() {
            
            bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bookmarks[currentBmIndex].endpoint);
        }

        private IEnumerator MoveToYLocation(bool lockOnFinish) {
            
            scrollRect.enabled = false;

            while (Vector2.Distance(bgRect.anchoredPosition, _mTargetPosition) > 5f) {
                
                var anchoredPosition = bgRect.anchoredPosition;

                Vector2 target = Vector2.MoveTowards(anchoredPosition, _mTargetPosition, forceScrollSpeed);
                
                anchoredPosition = new Vector2(anchoredPosition.x, target.y);
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
