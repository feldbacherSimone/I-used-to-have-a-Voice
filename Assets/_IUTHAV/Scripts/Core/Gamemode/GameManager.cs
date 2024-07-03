using System;
using System.Collections;
using _IUTHAV.Scripts.Core.Gamemode.CustomDataTypes;
using _IUTHAV.Scripts.Core.Input;
using _IUTHAV.Scripts.Core.Page;
using _IUTHAV.Scripts.Core.Scene;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Core.Gamemode {
#region Helper Classes

    [Serializable]
    public class GameStateBehaviour {
        
        public StateType stateType;
        public UnityEvent onFinish;

        [Header("Finish Behaviours")] [Tooltip("Will set any Data assigned to this state to null")]
        public bool isFreeze;
    }

#endregion

    public class GameManager : MonoBehaviour {
        private bool _isGamePaused;
        public bool IsGamePaused => _isGamePaused;

        [SerializeField] private GameStatesObject sceneGameStatesObject;
        [SerializeField] private GameStatesObject persistentGameStatesObject;

        [SerializeField] private GameStateBehaviour[] gameStateBehaviours;

        [SerializeField] private bool isDebug;

        private PageController _pageController;

        private Hashtable _mStates;

#region Unity Functions

        private void Awake() {
            
            Configure();
            //Update a sceneGameStateObject automatically
            if (sceneGameStatesObject != null) sceneGameStatesObject.GenerateList();
        }

        private void Start() {
            LogStates();
            InputController.Configure();
            FinishState(GetCurrentSceneType().ToString()+"_Start");
        }

        private void OnDestroy() {
            Dispose();
        }

        private void OnApplicationQuit() {
            QuiteGame();
        }

#endregion

#region Public Functions

        public StatePrefix? GetCurrentSceneType() {
            if (sceneGameStatesObject != null) {
                return sceneGameStatesObject.StatePrefix;
            }
            return null;
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

        public void QuiteGame() {
            InputController.Dispose();
        }

        public GameState GetState(StateType stateType) {
            if (!StateExists(stateType)) {
                LogWarning("State [" + stateType + "] does not exist!");
                return null;
            }

            return (GameState)_mStates[stateType];
        }

        public void SetStateData(StateType stateType, IFinishable finishable) {
            GameState gameState = GetState(stateType);

            if (gameState?.StateData != null) {
                LogWarning("Already assigned data to state [" + stateType + "]!");
                return;
            }

            gameState?.SetStateData(finishable);
        }

        public void UpdateStateData(StateType stateType, object obj) {
            GameState gameState = GetState(stateType);
            if (gameState?.StateData == null) {
                LogWarning("No data has been set to [" + stateType + "] yet. Using most generic class");
                SetStateData(stateType, new ObjData(obj, null));
            }

            gameState?.UpdateData(obj);
        }

        public void FinishState(StateType stateType) {
            GameState gameState = GetState(stateType);
            if (gameState != null) {
                gameState.Finish();
                Log("Finishing State [" + stateType + "]");
            }
            else {
                LogWarning("No GameState of Type [" + stateType + "] found!");
            }
        }

        [YarnCommand("finishState")]
        public void FinishState(string stateType) {
            if (Enum.TryParse(stateType, out StateType type)) {
                FinishState(type);
            }
            else {
                LogWarning("No GameState of Type [" + stateType + "] found!");
            }
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

            PopulateStatesTable();
            AssignDelegates();
            SceneLoader.Enable();
        }

        private void PopulateStatesTable() {
            _mStates = new Hashtable();

            if (persistentGameStatesObject != null) {
                
                foreach (GameState persistentState in persistentGameStatesObject.GameStates) {
                    RegisterState(persistentState);
                }
                
            }
            
            if (sceneGameStatesObject == null) return;
            
            foreach (GameState state in sceneGameStatesObject.GameStates) {
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
                GameState state = (GameState)_mStates[behaviour.stateType];

                if (state == null) {
                    Debug.LogError("Error evaluating state: " + behaviour.stateType + " try resetting it in Inspector!");
                }
                
                state.onStateCompleted = behaviour.onFinish;
                behaviour.onFinish = null;
                state.isFreeze = behaviour.isFreeze;

                state.Enable();
                
            }
        }

        private bool StateExists(StateType stateType) {
            return _mStates.Contains(stateType);
        }

        private void Dispose() {

            if (sceneGameStatesObject == null) return;
            
            foreach (GameState state in sceneGameStatesObject.GameStates) {
                state.Reset();
            }
        }

        public void LogStates() {
            Log("Current states: \n ------------");
            foreach (GameState state in _mStates.Values) {
                Log("    " + state + "\n");
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