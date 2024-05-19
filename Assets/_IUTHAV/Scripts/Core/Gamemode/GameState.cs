using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Core.Gamemode {
    
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
