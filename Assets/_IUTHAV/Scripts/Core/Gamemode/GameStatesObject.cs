using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Core.Gamemode {

    [CreateAssetMenu(fileName = "GameStatesObject", menuName = "ScriptableObjects/GameStatesObject", order = 2)]
    public class GameStatesObject : ScriptableObject {
        [Space(10)]
        [SerializeField] private bool resetStatesOnUnload = true;
        [SerializeField] private StatePrefix statePrefix;
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
}