using _IUTHAV.Core_Programming.Gamemode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _IUTHAV.Core_Programming.Utility {
    public class ReferenceManager : MonoBehaviour {

        public static ReferenceManager Instance;

        private GameManager _gameManager;
        private GameObject _singletonContainer;
        private Object[] _sessionDependables;
        
        private static GameManager _mGameManager;

        private const bool IsDebug = true;

#region Unity Functions

        private void Awake() {
            Configure();
        }

        private void OnDestroy() {
            DisposeObjects();
        }

#endregion

#region Public Functions

        /// <summary>
        /// Gives the current reference to this sessions Object "GameManager"
        /// </summary>
        /// <returns>May return null</returns>
        public static GameManager GameManager() {
            if (_mGameManager == null) {
                LogWarning("No GameManager found!");
            }
            return _mGameManager;
        }

#endregion
        
#region Private Functions

        public void Configure() {

            if (!Instance) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                AddSingletons();
                AddSessionDependables();
                EnableScriptableObjects();
            }
            else {
                Destroy(gameObject);
            }
            
        }

        private void AddSingletons() {
            _singletonContainer = Resources.Load<GameObject>("PersistentObjects/SingletonContainer");
            GameObject obj = Instantiate(_singletonContainer);
            DontDestroyOnLoad(obj);
        }

        private void AddSessionDependables() {

            _sessionDependables = Resources.LoadAll("PersistentObjects", typeof(ISessionDependable));
            
            
            
            foreach (var o in _sessionDependables) {
                if (o is GameManager obj) {
                    _mGameManager = obj;
                    return;
                }
            }
            LogWarning("No GameManager found in SessionDependables!");
        }

        private void EnableScriptableObjects() {
            foreach (var o in _sessionDependables) {
                var obj = (ISessionDependable)o;
                obj.Enable();
            }
        }

        private void DisposeObjects() {
            foreach (var o in _sessionDependables) {
                var obj = (ISessionDependable)o;
                obj.Reset();
            }
        }

        private static void LogWarning(string msg) {
           
            if (IsDebug) Debug.LogWarning("[SessionObjects] " + msg);
        }
        
#endregion

    }
}
