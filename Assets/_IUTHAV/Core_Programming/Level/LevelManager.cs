using _IUTHAV.Core_Programming.Gamemode;
using _IUTHAV.Core_Programming.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace _IUTHAV.Core_Programming.Level {
    public class LevelManager : MonoBehaviour {

        [SerializeField] private StateType stateType;
        [SerializeField] private Camera levelCamera;
        [SerializeField][Range(-100,100)] private float pageBorderPadding;

        public UnityEvent onLevelStart;
        public UnityEvent onLevelFinish;

        private StateData _levelData;
        private RectTransform _pageTransform;

#region Unity Functions

        private void Start() {
            onLevelStart.Invoke();
        }

#endregion

#region Public Functions

        public void UpdateLevelData(StateData stateData) {
            if (_levelData.currentScrollHeight < stateData.currentScrollHeight) {
                ChangePageBot(stateData.currentScrollHeight);
            }
            _levelData = stateData;
        }

        public void UpdateGameState() {
            ReferenceManager.GameManager().UpdateGameState(stateType, _levelData);
        }

        public void FinishLevel() {
            onLevelFinish.Invoke();
        }

#endregion

#region Private Functions

        private void ChangePageBot(float height) {
            var rect = _pageTransform.rect;
            _pageTransform.rect.Set(rect.x, rect.y, rect.width, height - pageBorderPadding);
        }

        private void ChangePageTop(float height) {
            
        }

#endregion

    }
}
