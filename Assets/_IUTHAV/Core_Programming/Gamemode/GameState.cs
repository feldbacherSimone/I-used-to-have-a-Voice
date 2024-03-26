using System;
using _IUTHAV.Core_Programming.Scenemanagement;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Gamemode {

    [Serializable]
    public class StateData {
    
        public int currentPanelId;
        
        /// <summary>
        /// Container for any State-specific data that must be saved within a session
        /// </summary>
        /// <param name="currentPanelId"></param>
        public StateData(int currentPanelId) {
            this.currentPanelId = currentPanelId;
        }
    }

    [Serializable]
    public class GameState {

        [SerializeField] private StateType stateType;
        public StateType StateType => stateType;

        [SerializeField] private SceneType sceneType;
        public SceneType SceneType => sceneType;

        [SerializeField] private StateType next;
        public StateType Next => next;

        public StateData stateData;


    }
}
