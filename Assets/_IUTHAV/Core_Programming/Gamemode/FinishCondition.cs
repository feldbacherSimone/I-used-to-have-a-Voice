using System.Collections;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Gamemode {
    public class FinishCondition : MonoBehaviour {
        
        [SerializeField] private StateType[] observedStates;
        [SerializeField] private StateType affectedState;

        [SerializeField] private bool isDebug;

        private Hashtable _mObservedStates;
        private GameManager _gameManager;

#region Unity Functions

        private void Awake() {
            _mObservedStates = new Hashtable();
            _gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        }
        
        private void Start() {
            Configure();
        }

        private void OnDisable() {
            Dispose();
        }

#endregion

#region Private Functions

        private void CheckCondition(GameState state = null) {
            
            Log("Observed change in [" + state?.StateType + "], data is [" + state?.StateData + "]");

            bool met = true;
            foreach (GameState gameState in _mObservedStates.Values) {
                met &= gameState.IsFinished;
            }

            if (met) _gameManager.GetState(affectedState).Finish();
        }

        private void PopulateObservedStatesTable() {
        
            foreach (StateType stateType in observedStates) {
                Log("Getting state [" + stateType + "]");
                GameState gameState = _gameManager.GetState(stateType);
                _mObservedStates.Add(stateType, gameState);
            }
        }

        private void Configure() {
            PopulateObservedStatesTable();

            foreach (GameState state in _mObservedStates.Values) {
                state.OnDataChanged += CheckCondition;
            }
            
            CheckCondition();
        }

        private void Dispose() {

            foreach (GameState state in _mObservedStates.Values) {
                state.OnDataChanged -= CheckCondition;
            }
            
        }
        
        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[StateCondition] " + msg);
        }

#endregion
        
    }
}
