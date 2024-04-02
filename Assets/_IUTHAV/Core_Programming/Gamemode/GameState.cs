using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _IUTHAV.Core_Programming.Gamemode {

    [CreateAssetMenu(fileName = "GameStatesObject", menuName = "ScriptableObjects/GameStatesObject", order = 2)]
    public class GameStatesObject : ScriptableObject {

        [SerializeField] private GameState[] gameStates;
        public GameState[] GameStates => gameStates;

        private void Awake() {
            
        }

    }

    [Serializable]
    public class GameState {

        [SerializeField] private StateType stateType;
        public StateType StateType => stateType;

        public delegate void DataChanged(GameState gameState);
        public event DataChanged OnDataChanged;
        [HideInInspector] public UnityEvent onStateCompleted;
        private IFinishable _stateData;
        public IFinishable StateData => _stateData;
        
        [SerializeField] private bool resetOnUnload = true;

        [SerializeField] private bool isFinished;
        public bool IsFinished => isFinished;

        public void Enable() {
            
            InvokeDataChangedEvent();
            if (isFinished) onStateCompleted.Invoke();
        }

        public void SetStateData(IFinishable finishable) {
            _stateData = finishable;
        }
        public void UpdateData(object data) {
            if (!isFinished) {

                if (_stateData == null) return;
                
                _stateData.UpdateData(data);

                isFinished = _stateData.CheckFinishCondition(); 
                
                //Check if state was set to finished by a condition
                InvokeDataChangedEvent();
                if (isFinished) {
                    onStateCompleted.Invoke();
                }
            }
        }
        
        public void Finish() {
            if (!isFinished) {
                isFinished = true;
                InvokeDataChangedEvent();
                onStateCompleted.Invoke();
            }
        }

        public override string ToString() {
            return (stateType + " | OnComplete Methodname: " +
                    onStateCompleted?.GetPersistentMethodName(0) + " | StateData: " + StateData + " | isFinished: " + isFinished);
        }

        public void Reset(bool forceReset = false) {
            onStateCompleted = new UnityEvent();

            if (resetOnUnload || forceReset) {
                _stateData = _stateData?.Reset();
                isFinished = false;
            }
        }

        private void InvokeDataChangedEvent() {
            OnDataChanged?.Invoke(this);
        }
        
    }
}
