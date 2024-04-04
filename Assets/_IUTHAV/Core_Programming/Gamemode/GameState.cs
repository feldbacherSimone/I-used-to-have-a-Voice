using System;
using System.Collections.Generic;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _IUTHAV.Core_Programming.Gamemode {

#region ScriptableObject Implementation

    [CreateAssetMenu(fileName = "GameStatesObject", menuName = "ScriptableObjects/GameStatesObject", order = 2)]
    public class GameStatesObject : ScriptableObject {
        [Space(10)]
        [SerializeField] private bool resetStatesOnUnload = true;
        [FormerlySerializedAs("prefix")] [FormerlySerializedAs("dataType")] [SerializeField] private StatePrefix statePrefix;
        public StatePrefix StatePrefix => statePrefix;
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
                switch (statePrefix) {
                    case StatePrefix.PER:
                        idString = "PER";
                        break;
                    case StatePrefix.SC1:
                        idString = "SC1";
                        break;
                    case StatePrefix.SC2:
                        idString = "SC2";
                        break;
                    case StatePrefix.SC3:
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

#endregion
    
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
        [HideInInspector] public bool isFreeze;

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

                if (isFreeze) {
                    _stateData = null;
                }
                
            }
        }

        public void UnFinish() {
            if (isFinished && !isFreeze) {
                isFinished = false;
                InvokeDataChangedEvent();
            }
        }

        public override string ToString() {

            string eventListener = (onStateCompleted?.GetPersistentEventCount() > 0)
                ? onStateCompleted.GetPersistentMethodName(0)
                : "None";
                
            return (stateType + " | OnComplete Methodname: " +
                    eventListener + " | StateData: " + StateData + " | isFinished: " + isFinished);
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
