using System;
using System.Collections;
using System.Threading.Tasks;
using _IUTHAV.Core_Programming.Page;
using _IUTHAV.Core_Programming.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _IUTHAV.Core_Programming.Scene {
    
    public class SceneController : MonoBehaviour {

        public static SceneController Instance;

        [SerializeField] private bool isDebug;

        private const int LoadTime = 1500;
        
        private SceneType _mTargetScene;
        private PageType _mLoadingPage;
        private bool _mIsSceneLoading;
        private PageController _pageController;
        private LoadSceneMode _mLoadSceneMode;
        
        private string CurrentSceneName => SceneManager.GetActiveScene().name;

#region Unity Functions

        private void Awake() {
            Configure();
        }

        private void OnDisable() {
            Dispose();
        }

#endregion

#region Public Functions

        /// <summary>
        /// Load a desired scene, available by choice from the SceneType enum. Ensure it´s included in the Build settings
        /// </summary>
        /// <param name="loadParameters">Container of parameters, that handle the behaviour of the scene load</param>
        public void Load(SceneLoadParameters loadParameters) {

            if (loadParameters.loadingPage != PageType.None && PageController.Instance != null) {
                LogWarning("No PageController found when trying to load loadingscreen!");
                return;
            }
            if (!SceneCanBeLoaded(loadParameters.sceneType, loadParameters.reload)) {
                return;
            }

            _mIsSceneLoading = true;
            _mTargetScene = loadParameters.sceneType;
            _mLoadingPage = loadParameters.loadingPage;
            _mLoadSceneMode = loadParameters.loadMode;

            StartCoroutine("LoadScene");
        }

        public void UnloadScene(SceneType sceneType) {
            SceneManager.UnloadSceneAsync(sceneType.ToString());
        }

        public void UnloadScene(string sceneName) {
            SceneManager.UnloadSceneAsync(sceneName);
        }



#endregion

#region Private Functions

        private void Configure() {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Log("Configured and ready");
        }

        private async void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {

            if (_mTargetScene == SceneType.None) {
                return;
            }

            if (!Enum.TryParse(scene.name, out SceneType sceneType)) {
                LogWarning("Loaded Scene is not a SceneType!");
                return;
            }

            await Task.Delay(LoadTime);
            _pageController.TurnPageOff(_mLoadingPage);
        }

        private bool SceneCanBeLoaded(SceneType sceneType, bool reload) {
        
            if (CurrentSceneName == sceneType.ToString() && !reload) {
                LogWarning("Scene ["+sceneType+"] already active");
                return false;
            } else if (sceneType.ToString() == string.Empty) {
                LogWarning("Empty scene name");
                return false;
            } else if (_mIsSceneLoading) {
                LogWarning("Cannot load scene ["+sceneType+"], another one is already loading");
                return false;
            }
            return true;
        }

        private IEnumerator LoadScene() {

            if (_mLoadingPage != PageType.None) {
                _pageController.TurnPageOn(_mLoadingPage);
                while (!_pageController.PageIsOn(_mLoadingPage)) {
                    yield return null;
                }
            }

            SceneManager.LoadScene(_mTargetScene.ToString(), _mLoadSceneMode);
        }

        private void Dispose() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[SceneController] " + msg);
        }
        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[SceneController] " + msg);
        }

#endregion

    }
    
#region Helper Classes

        [Serializable]
        public class SceneLoadParameters {
        
            public SceneType sceneType;
            public PageType loadingPage = PageType.LoadingPage;
            public LoadSceneMode loadMode = LoadSceneMode.Single;
            public bool reload;

            /// <summary>
            /// Container to store parameters used to load a scene
            /// </summary>
            /// <param name="sceneType">Will be converted to a string and sent to SceneManager. Typename must match scenename!</param>
            /// <param name="loadingPage">Page that will be turned on/off while loading the scene - can be PageType.None</param>
            /// <param name="loadMode"></param>
            /// <param name="reload">Use, if the sceneType you're loading matches the current scene you're in</param>
            public SceneLoadParameters(SceneType sceneType, PageType loadingPage = PageType.LoadingPage, LoadSceneMode loadMode = LoadSceneMode.Single, bool reload = false) {
                this.sceneType = sceneType;
                this.loadingPage = loadingPage;
                this.loadMode = loadMode;
                this.reload = reload;
            }
        }

#endregion
}
