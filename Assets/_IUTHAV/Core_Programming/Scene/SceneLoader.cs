using System;
using System.Collections.Generic;
using _IUTHAV.Core_Programming.Page;
using UnityEngine;
using UnityEngine.SceneManagement;
using Task = System.Threading.Tasks.Task;

namespace _IUTHAV.Core_Programming.Scene {

    public static class SceneLoader {
        
        public static readonly string FLAG_ON = "On";
        public static readonly string FLAG_OFF = "Off";
        public static readonly string FLAG_NONE = "None";

        private static string _currentLoadingState = FLAG_NONE;
        private static Dictionary<SceneType, LoadingParameters> _loadList;
        
        private const bool IsDebug = true;

#region Public Functions

        public static void Enable() {
            if (_currentLoadingState == FLAG_NONE) {
                SceneManager.sceneLoaded += OnSceneLoaded;
                _currentLoadingState = FLAG_OFF;
            }
        }

        public static void Disable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _currentLoadingState = FLAG_NONE;
        }

        public static void LoadSingle(SceneType sceneType, PageType loadingPage = PageType.None, int loadTime = 600) {

            if (!Enum.TryParse(SceneManager.GetActiveScene().name, out SceneType currentSceneType)) {
                LogWarning("Loaded Scene is not a SceneType!");
                return;
            }
            _loadList.Remove(currentSceneType);
            _loadList.Add(sceneType, new LoadingParameters(loadingPage, LoadSceneMode.Single, loadTime));
            
            LoadScene(sceneType);
        }

        public static void LoadAdditive(SceneType sceneType, PageType loadingPage = PageType.None, int loadTime = 600) {
            
            _loadList.Add(sceneType, new LoadingParameters(loadingPage, LoadSceneMode.Additive, loadTime));
            LoadScene(sceneType);
        }

        public static void UnloadScene(SceneType sceneType) {

            _loadList.Remove(sceneType);
            SceneManager.UnloadSceneAsync(sceneType.ToString());
        }
        
        public static void LogActiveSceneNames() {
            Log("Currently active scenes: ", true);
            foreach (SceneType type in _loadList.Keys) {
                Log(type.ToString() + " ", true);
            }
        }

#endregion

#region Private Functions

        private static void LoadScene(SceneType type) {
        
            if (_currentLoadingState == FLAG_ON) {
                LogWarning("Cannot load a scene while another is currently loading");
                return;
            }
            _currentLoadingState = FLAG_ON;
            
            if (_loadList[type].loadingPage != PageType.None) {
                PageController.Instance.TurnPageOn(_loadList[type].loadingPage);
            }
            SceneManager.LoadScene(type.ToString(), _loadList[type].loadSceneMode);
        }

        private static async void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {
            
            if (!Enum.TryParse(scene.name, out SceneType sceneType)) {
                LogWarning("Loaded Scene is not a SceneType!");
                _currentLoadingState = FLAG_OFF;
                return;
            }
            await Task.Delay(_loadList[sceneType].loadTime);

            if (_loadList[sceneType].loadingPage != PageType.None) {
                PageController.Instance.TurnPageOff(_loadList[sceneType].loadingPage);
            }
            _currentLoadingState = FLAG_OFF;
        }

        private static void Log(string msg, bool forceDebug = false) {
            if (IsDebug || forceDebug) Debug.Log("[SceneLoader] " + msg);
        }
        
        private static void LogWarning(string msg) {
            if (!IsDebug) return;
            Debug.LogWarning("[SceneLoader] " + msg);
        }

#endregion

#region Helper Classes

        public class LoadingParameters {
        
            public readonly PageType loadingPage;
            public LoadSceneMode loadSceneMode;
            public readonly int loadTime;

            public LoadingParameters(PageType loadingPage, LoadSceneMode loadSceneMode, int loadTime) {
                this.loadingPage = loadingPage;
                this.loadSceneMode = loadSceneMode;
                this.loadTime = loadTime;
            }
        }

#endregion
    }
}
