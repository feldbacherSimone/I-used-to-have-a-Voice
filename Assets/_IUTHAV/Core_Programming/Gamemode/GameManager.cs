using _IUTHAV.Core_Programming.Page;
using _IUTHAV.Core_Programming.Scene;
using _IUTHAV.Core_Programming.Utility;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Gamemode {
    [CreateAssetMenu(fileName = "GameManager", menuName = "ScriptableObjects/GameManager", order = 2)]

    public class GameManager : ScriptableObject, ISessionDependable {
    
        private bool _isGamePaused;
        public bool IsGamePaused => _isGamePaused;

        [SerializeField] private GameState currentState;
        public GameState CurrentState => currentState;

        [SerializeField] private GameState[] gameStates;

        [SerializeField] private bool isDebug;

        private SceneController _sceneController;
        private PageController _pageController;

#region Public Functions

        public void Enable() {
            Configure();
        }

        public void Reset() {
            throw new System.NotImplementedException();
        }

        public void LoadGameState() {
            
            if (currentState != null && currentState.StateType != StateType.None) {
                Log("Starting Game with state [" + currentState.StateType + "] scene [" + currentState.SceneType + "]");
                _sceneController.Load(new SceneLoadParameters(currentState.SceneType));
            }
        }

        public void PauseGame(bool pause) {
        
            if (pause) {
                _pageController.TurnPageOn(PageType.PausePage);
            }
            else {
                _pageController.TurnPageOff(PageType.PausePage);
            }
            _isGamePaused = pause;
        }

        public void AdvanceState() {
        
            if (currentState != null && currentState.StateType != StateType.None) {
                currentState = GetGameState(currentState.Next);
            }
            else {
                LogWarning("Cannot update, current state is null!");
            }
        }

        public void UpdateCurrentGameState(StateData stateData) {
            if (currentState != null && currentState.StateType != StateType.None) {
                currentState.stateData = stateData;
            }
            else {
                LogWarning("Cannot update, current state is null!");
            }
        }

        public void UpdateGameState(StateType stateType, StateData stateData) {
            GameState gameState = GetGameState(stateType);
            if (gameState != null) gameState.stateData = stateData;
        }

        public void GoToMainMenu() {
            _sceneController.Load(new SceneLoadParameters(SceneType.MainMenu));
        }

#endregion

#region Private Functions

        private void Configure() {
            _sceneController = SceneController.Instance;
            _pageController = PageController.Instance;

            if (currentState == null) currentState = gameStates[0];
            
            Log("Configured and ready");
        }

        private GameState GetGameState(StateType stateType) {
            foreach (GameState gameState in gameStates) {
                if (gameState.StateType == stateType) {
                    return gameState;
                }
            }
            LogWarning("No GameState of type [" + stateType + "] found!");
            return null;
        }
        
        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[GameManager] " + msg);
        }
        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[GameManager] " + msg);
        }

#endregion
        
    }
}
