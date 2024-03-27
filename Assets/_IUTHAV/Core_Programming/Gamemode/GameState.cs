using System;
using _IUTHAV.Core_Programming.Scene;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Gamemode {

    [Serializable]
    public class StateData {
    
        public int currentPanelId;
        public float currentScrollHeight;

        /// <summary>
        /// Container for any State-specific data that must be saved within a session
        /// </summary>
        /// <param name="currentPanelId"></param>
        /// <param name="currentScrollHeight"></param>
        public StateData(int currentPanelId, float currentScrollHeight) {
            this.currentPanelId = currentPanelId;
            this.currentScrollHeight = this.currentScrollHeight;
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
