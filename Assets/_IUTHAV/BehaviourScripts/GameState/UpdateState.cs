using _IUTHAV.Core_Programming.Gamemode.CustomDataTypes;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Gamemode {
    public class UpdateState : MonoBehaviour {

        [SerializeField] private StateType stateType;

        private GameManager _gameManager;

        private void Awake() {
            
            _gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        }

        public void FinishState() {
        
            _gameManager.FinishState(stateType);
        }

        public void UnFinishState() {
            
            _gameManager.GetState(stateType).UnFinish();
        }
        
    }
}
