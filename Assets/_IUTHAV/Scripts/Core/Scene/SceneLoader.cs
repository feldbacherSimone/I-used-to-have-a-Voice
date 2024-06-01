using System.Collections.Generic;
using _IUTHAV.Scripts.Core.Page;
using UnityEngine;
using UnityEngine.SceneManagement;
using Task = System.Threading.Tasks.Task;

namespace _IUTHAV.Scripts.Core.Scene {

    public static class SceneLoader {
        
        public static readonly string FLAG_ON = "On";
        public static readonly string FLAG_OFF = "Off";
        public static readonly string FLAG_NONE = "None";
        
        private static string _currentLoadingState = FLAG_NONE;
        private static Dictionary<string, LoadingParameters> _loadList;
        
        private const bool IsDebug = true;
        private static bool IsReady;

#region Public Functions

        public static void Enable() {

            if (IsReady) return;
            
            _loadList = new Dictionary<string, LoadingParameters>();
            _loadList.Add(SceneManager.GetActiveScene().name, new LoadingParameters(PageType.None, LoadSceneMode.Single));
            if (_currentLoadingState == FLAG_NONE) {
                SceneManager.sceneLoaded += OnSceneLoaded;
                _currentLoadingState = FLAG_OFF;
            }

            IsReady = true;
        }

        public static void Disable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _currentLoadingState = FLAG_NONE;
        }

        public static void LoadSingle(string sceneType, PageType loadingPage = PageType.None) {
            
            _loadList.Remove(SceneManager.GetActiveScene().name);
            _loadList.Add(sceneType, new LoadingParameters(loadingPage, LoadSceneMode.Single));
            LoadScene(sceneType);
        }

        public static void LoadAdditive(string sceneType, PageType loadingPage = PageType.None) {
            
            _loadList.Add(sceneType, new LoadingParameters(loadingPage, LoadSceneMode.Additive));
            LoadScene(sceneType);
        }

        public static void UnloadScene(string sceneType) {

            _loadList.Remove(sceneType);
            SceneManager.UnloadSceneAsync(sceneType.ToString());
        }
        
        public static void LogActiveSceneNames() {
            Log("Currently active scenes: ", true);
            foreach (string type in _loadList.Keys) {
                Log(type.ToString() + " ", true);
            }
        }

#endregion

#region Private Functions

        private static void LoadScene(string type) {
        
            if (_currentLoadingState == FLAG_ON) {
                LogWarning("Cannot load a scene while another is currently loading");
                return;
            }
            _currentLoadingState = FLAG_ON;

            SceneManager.LoadScene(type.ToString(), _loadList[type].loadSceneMode);
        }

        private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {
            
            Log("Load time completed");
            _currentLoadingState = FLAG_OFF;
        }

        private static void Log(string msg, bool forceDebug = false) {
            if (IsDebug || forceDebug) Debug.Log("[SceneLoader] " + msg);
        }
        
        private static void LogWarning(string msg) {
            if (IsDebug)Debug.LogWarning("[SceneLoader] " + msg);
        }

#endregion

#region Helper Classes

        public class LoadingParameters {
        
            public readonly PageType loadingPage;
            public LoadSceneMode loadSceneMode;

            public LoadingParameters(PageType loadingPage, LoadSceneMode loadSceneMode) {
                this.loadingPage = loadingPage;
                this.loadSceneMode = loadSceneMode;
            }
        }

#endregion
    }
}
