using System;
using _IUTHAV.Core_Programming.Gamemode;
using _IUTHAV.Core_Programming.Gamemode.CustomDataTypes;
using _IUTHAV.Core_Programming.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace _IUTHAV.BehaviourScripts {

#region Helper Classes
    
    [Serializable]
    public class Bookmark {
        public float trigger;
        public float endpoint;
        [Tooltip("Set to make the trigger finish a specified state. Will FREEZE that state upon finish!")]
        public StateType customState = StateType.None;
        
    }

#endregion
    
    public class ScrollBackGround : MonoBehaviour {

        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform bgRect;
        [SerializeField] private int currentBmIndex;
        [SerializeField] private Bookmark[] bookmarks;

        [Space(10)] [SerializeField] private bool isDebug;

        private GameManager _mGameManager;
        private GameState _mCurrentTriggeredState;
        private float _mBgOffset;

#region Unity Functions

        private void OnDrawGizmos() {

            Gizmos.matrix = canvasRect.localToWorldMatrix;
            
            Gizmos.color = Color.red;
            foreach (var f in bookmarks) {
                var localPosition = bgRect.localPosition;
                var rect = bgRect.rect;
                Gizmos.DrawLine(new Vector3(-rect.width/2, -f.endpoint + localPosition.y), new Vector3(rect.width/2, -f.endpoint + localPosition.y));
            }
            Gizmos.color = Color.yellow;
            foreach (var f in bookmarks) {
                var localPosition = bgRect.localPosition;
                var rect = bgRect.rect;
                Gizmos.DrawLine(new Vector3(-rect.width/2, -f.trigger + localPosition.y), new Vector3(-rect.width/2.5f, -f.trigger + localPosition.y));
                Gizmos.DrawLine(new Vector3(rect.width/2.5f, -f.trigger + localPosition.y), new Vector3(rect.width/2, -f.trigger + localPosition.y));
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

            FinishCurrentActState();

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

#endregion

#region Private Functions

        private void SetTriggeredStateData() {
            if (bookmarks[currentBmIndex].customState == StateType.None) {
                StateType contextSensitiveState = Typeconverter.ChangePreAndSuffix(_mGameManager.GetCurrentSceneType(), StateType.SC1_ReachedScrollTrigger);
                _mCurrentTriggeredState = _mGameManager.GetState(contextSensitiveState);
            }
            else {
                _mCurrentTriggeredState = _mGameManager.GetState(bookmarks[currentBmIndex].customState);
                _mCurrentTriggeredState.isFreeze = true;
            }
            _mCurrentTriggeredState.SetStateData(new FloatData(
                bgRect.localPosition.y,
                bookmarks[currentBmIndex].trigger - _mBgOffset
                ));
        }

        private void UpdateScrollStateData(Vector2 pos) {
            
            _mCurrentTriggeredState.UpdateData(bgRect.localPosition.y);
        }

        private void FinishCurrentActState() {

            Log("Finishing current Act [" + (currentBmIndex) + "]");
            
            StateType contextSensitiveState = Typeconverter.ChangePreAndSuffix(
                _mGameManager.GetCurrentSceneType(), 
                StateType.SC1_ActFinished_0,
                currentBmIndex
            );
            
            _mGameManager.GetState(contextSensitiveState).Finish();
        }

        private void StrechBackground() {

            bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bookmarks[currentBmIndex].endpoint + 1);
        }

        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[ScrollBackground] " + msg);
        }
        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[ScrollBackground] " + msg);
        }

#endregion


    }



}
