using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _IUTHAV.Core_Programming.Gamemode {

    [CreateAssetMenu(fileName = "GameStatesObject", menuName = "ScriptableObjects/GameStatesObject", order = 2)]
    public class GameStatesObject : ScriptableObject {

        private void Awake() {
            //set each gamestates type to the assigned type in the editor
            
        }

        [SerializeField] private GameState[] gameStates;
        public GameState[] GameStates => gameStates;

    }

    [Serializable]
    public class GameState {

        [SerializeField] private StateType stateType;
        public StateType StateType => stateType;

        public delegate void DataChanged(GameState gameState);
        public event DataChanged OnDataChanged;
        [HideInInspector] public UnityEvent onStateCompleted;
        [SerializeField] private IFinishable _stateData;
        [SerializeField] private SerializableType type;
        public IFinishable StateData => _stateData;
        
        [SerializeField] private bool persistState;

        private bool _isFinished;
        public bool IsFinished => _isFinished;

        public void UpdateData(IFinishable data) {
            if (!_isFinished) {

                _stateData.UpdateData(data.GetData());
                _stateData.CheckFinishCondition();
                InvokeDataChangedEvent();
                
                //Check if state was set to finished by a condition
                if (_isFinished) {
                    onStateCompleted.Invoke();
                }
            }
        }
        
        public void Finish() {
            if (!_isFinished) {
                _isFinished = true;
                InvokeDataChangedEvent();
                onStateCompleted.Invoke();
            }
        }

        public override string ToString() {
            return (stateType.ToString() + " | " + OnDataChanged?.ToString() + " | " +
                    onStateCompleted.GetPersistentMethodName(0) + " | " + _isFinished);
        }

        public void Reset() {
            onStateCompleted = new UnityEvent();

            if (!persistState) {
                _stateData = _stateData.Reset();
                _isFinished = false;
            }
        }

        private void InvokeDataChangedEvent() {
            OnDataChanged?.Invoke(this);
        }
        
    }
}
