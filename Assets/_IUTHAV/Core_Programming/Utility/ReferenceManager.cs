using _IUTHAV.Core_Programming.Gamemode;
using _IUTHAV.Core_Programming.Page;
using _IUTHAV.Core_Programming.Scene;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Utility {
    public class ReferenceManager : MonoBehaviour {

        public static ReferenceManager Instance;
        
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject singletonContainer;

        [SerializeField] private bool isDebug;
 
        private static GameManager _gameManager;
        private static SceneController _sceneController;
        private static PageController _pageController;

        private static bool _isDebug;

#region Unity Functions

        private void Awake() {
            Configure();
        }

#endregion

#region Public Functions

        /// <summary>
        /// Gives the current reference to this sessions Object "GameManager"
        /// </summary>
        /// <returns>May return null</returns>
        public static GameManager GameManager() {
            if (_gameManager == null) {
                LogWarning("No GameManager found!");
            }
            return _gameManager;
        }
        
        /// <summary>
        /// Gives the current reference to this sessions Object "SceneController"
        /// </summary>
        /// <returns>May return null</returns>
        public static SceneController SceneController() {
            if (_sceneController == null) {
                LogWarning("No SceneController found!");
            }
            return _sceneController;
        }

        /// <summary>
        /// Gives the current reference to this sessions Object "PageController"
        /// </summary>
        /// <returns>May return null</returns>
        public static PageController PageController() {
            if (_pageController == null) {
                LogWarning("No PageController found!");
            }
            return _pageController;
        }
        
#endregion
        
#region Private Functions

        public void Configure() {

            if (!Instance) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                AddSingletons();
                ConfigureStaticReferences();
                GameManager().Enable();
            }
            else {
                Destroy(gameObject);
            }
            
        }

        private void AddSingletons() {

            GameObject obj = Instantiate(singletonContainer);
            DontDestroyOnLoad(obj);
        }

        private void ConfigureStaticReferences() {
            _gameManager = gameManager;
            _sceneController = Scene.SceneController.Instance;
            _pageController = Page.PageController.Instance;
            _isDebug = isDebug;
        }

        private static void LogWarning(string msg) {
            if (!_isDebug) return;
            Debug.LogWarning("[SessionObjects] " + msg);
        }
        
#endregion

    }
}
