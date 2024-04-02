using System;
using System.Collections;
using _IUTHAV.Core_Programming.Page;
using UnityEngine;
using UnityEngine.Events;

namespace _IUTHAV.Core_Programming.Gamemode {
    
#region Helper Classes

    [Serializable]
    public class GameStateBehaviour {
        public StateType stateType;
        public UnityEvent onFinish;
    }

#endregion

    public class GameManager : MonoBehaviour {
    
        private bool _isGamePaused;
        public bool IsGamePaused => _isGamePaused;
        
        [SerializeField] private GameStatesObject gameStates;

        [SerializeField] private GameStateBehaviour[] gameStateBehaviours;
        
        [SerializeField] private bool isDebug;

        private PageController _pageController;

        private Hashtable _mStates;

#region Unity Functions

        private void Awake() {
            Configure();
        }

        private void OnDestroy() {
            Dispose();
        }

#endregion

#region Public Functions

        public void PauseGame(bool pause) {
        
            if (pause) {
                _pageController.TurnPageOn(PageType.PausePage);
            }
            else {
                _pageController.TurnPageOff(PageType.PausePage);
            }
            _isGamePaused = pause;
        }
        
        public GameState GetState(StateType stateType) {
            if (!StateExists(stateType)) {
                LogWarning("State [" + stateType + "] does not exist!");
                return null;
            }
            return (GameState)_mStates[stateType];
        }

        public void UpdateState(StateType stateType, IFinishable finishable) {
            GameState gameState = GetState(stateType);
            if (gameState != null) gameState.UpdateData(finishable);
        }

        public void AddState(GameState state) {
            RegisterState(state);
        }

        public void RemoveState(StateType stateType) {
            DeRegisterState(stateType);
        }

#endregion

#region Private Functions

        private void Configure() {
            _pageController = PageController.Instance;
            PopulateStatesTable();
            AssignDelegates();
            LogStates();
        }

        private void PopulateStatesTable() {
            _mStates = new Hashtable();
            foreach (GameState state in gameStates.GameStates) {

                RegisterState(state);
            }
            
        }

        private void RegisterState(GameState state) {
            if (StateExists(state.StateType)) {
                LogWarning("Cannot register [" + state.StateType + "] because it already exists!");
            }
            else {
                _mStates.Add(state.StateType, state);
                Log("Registered state [" + state.StateType + "]");
            }
        }
        
        private void DeRegisterState(StateType stateType) {
            if (!StateExists(stateType)) {
                LogWarning("Cannot delete [" + stateType +
                           "] because it´s not registered");
                return;
            }
            _mStates.Remove(stateType);
            Log("Deleted state [" + stateType + "]");
        }

        private void AssignDelegates() {
            foreach (GameStateBehaviour behaviour in gameStateBehaviours) {
                //Does this work? Idunno, lets find out
                ((GameState)_mStates[behaviour.stateType]).onStateCompleted = behaviour.onFinish;
                Log(((GameState)_mStates[behaviour.stateType]).onStateCompleted.GetPersistentMethodName(0));
                behaviour.onFinish = null;
            }
        }

        private bool StateExists(StateType stateType) {
            return _mStates.Contains(stateType);
        }

        private void Dispose() {

            foreach (GameState state in gameStates.GameStates) {
                state.Reset();
            }
        }

        private void LogStates() {
            Log("Current states: \n ------------");
            foreach (GameState state in _mStates.Values) {
                Log("    " + state.ToString() + "\n");
            }
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
