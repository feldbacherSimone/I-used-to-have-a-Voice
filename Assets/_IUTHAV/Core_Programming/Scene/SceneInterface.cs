using _IUTHAV.Core_Programming.Utility;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Scene {

    [CreateAssetMenu(fileName = "SceneInterface", menuName = "ScriptableObjects/SceneInterface", order = 3)]
    public class SceneInterface : ScriptableObject, ISessionDependable {

        [SerializeField] private SceneLoadParameters[] loadActions;

        [SerializeField] private bool isDebug;
        
        private SceneController _sceneController;

        public void Enable() {
            _sceneController = SceneController.Instance;
            Log("Enabled and ready");
        }

        public void Reset() {
            _sceneController = null;
        }

        public void Load(int loadActionElementIndex) {
            if (_sceneController != null) {
                _sceneController.Load(loadActions[loadActionElementIndex]);
            }
            else {
                LogWarning("No SceneController set!");
            }
        }

        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[SceneInterface] " + msg);
        }
        
        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[SceneInterface] " + msg);
        }

    }
}
