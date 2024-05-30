using System;
using System.Collections.Generic;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.Events;

namespace _IUTHAV.Core_Programming.Gamemode {

    [CreateAssetMenu(fileName = "GameStatesObject", menuName = "ScriptableObjects/GameStatesObject", order = 2)]
    public class GameStatesObject : ScriptableObject {
        [Space(10)]
        [SerializeField] private bool resetStatesOnUnload = true;
        [SerializeField] private DataType dataType;
        [Space(10)]
        [SerializeField] private List<GameState> gameStates;
        [Space(10)]
        [SerializeField] private bool regenerateList;
        public List<GameState> GameStates => gameStates;

        private void OnValidate() {
            if (regenerateList) {
                GenerateList();
                regenerateList = false;
            }
        }

        public void GenerateList() {
            
            gameStates.Clear();
            
            string idString = "PER";
                switch (dataType) {
                    case DataType.Persistent:
                        idString = "PER";
                        break;
                    case DataType.Scene1:
                        idString = "SC1";
                        break;
                    case DataType.Scene2:
                        idString = "SC2";
                        break;
                    case DataType.Scene3:
                        idString = "SC3";
                        break;
                }

            foreach (StateType type in Enum.GetValues(typeof(StateType))) {

                if (type.ToString().StartsWith(idString)) {
                
                    gameStates.Add(new GameState(type, resetStatesOnUnload));
                }

            }
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

        [HideInInspector] public bool resetOnUnload = true;

        [SerializeField] private bool isFinished;
        public bool IsFinished => isFinished;

        public GameState(StateType stateType, bool resetOnUnload) {
            this.stateType = stateType;
            this.resetOnUnload = resetOnUnload;
        }

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
