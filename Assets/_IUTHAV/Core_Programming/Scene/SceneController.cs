using System;
using System.Collections;
using System.Threading.Tasks;
using _IUTHAV.Core_Programming.Page;
using _IUTHAV.Core_Programming.Scenemanagement;
using _IUTHAV.Core_Programming.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _IUTHAV.Core_Programming.Scene {
    public class SceneController : MonoBehaviour {
    
        public delegate void SceneLoadDelegate(SceneType scene);

        public static SceneController Instance;

        [SerializeField] private bool isDebug;

        private const int LoadTime = 1500;
        
        private SceneType _mTargetScene;
        private PageType _mLoadingPage;
        private SceneLoadDelegate _mSceneLoadDelegate;
        private bool _mIsSceneLoading;
        private PageController _pageController;
        
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
        /// <param name="sceneType">Will be converted into a string and Loaded by Unitys SceneManager</param>
        /// <param name="loadingPage">Optional loading Page that is blended between switching scenes</param>
        /// <param name="reload">If you want to reload the current scene you're in, set this to true</param>
        /// <param name="sceneLoadDelegate">Delegate, that is called once loading starts</param>
        public void Load(SceneType sceneType, PageType loadingPage = PageType.None, bool reload = false, SceneLoadDelegate sceneLoadDelegate = null) {

            if (loadingPage != PageType.None && ReferenceManager.PageController() != null) {
                LogWarning("No PageController found when trying to load loadingscreen!");
                return;
            }
            if (!SceneCanBeLoaded(sceneType, reload)) {
                return;
            }

            _mIsSceneLoading = true;
            _mTargetScene = sceneType;
            _mLoadingPage = loadingPage;
            _mSceneLoadDelegate = sceneLoadDelegate;

            StartCoroutine("LoadScene");
        }

        public void Load(string sceneType, PageType loadingPage = PageType.None, bool reload = false, SceneLoadDelegate sceneLoadDelegate = null) {
            if (Enum.TryParse(sceneType, out SceneType type)) {
                Load(type, loadingPage, reload, sceneLoadDelegate);
            }
            else {
                LogWarning("No scene of type [" + sceneType + "] found!");
            }
        }

#endregion

#region Private Functions

        private void Configure() {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private async void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {

            if (_mTargetScene == SceneType.None) {
                return;
            }

            if (!Enum.TryParse(scene.name, out SceneType sceneType)) {
                LogWarning("Loaded Scene is not a SceneType!");
                return;
            }

            if (_mSceneLoadDelegate != null) {
                try {
                    _mSceneLoadDelegate(sceneType);
                }
                catch (SystemException) {
                    LogWarning("Unable to respond with sceneLoadDelegate after scene ["+sceneType+"] loaded");
                }
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

            SceneManager.LoadScene(_mTargetScene.ToString());
        }

        private void Dispose() {
            
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
}
