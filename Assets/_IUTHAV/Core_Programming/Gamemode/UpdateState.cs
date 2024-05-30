using _IUTHAV.Core_Programming.Gamemode.CustomDataTypes;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Gamemode {
    public class UpdateState : MonoBehaviour {

        [SerializeField] private StateType stateType;

        private GameManager _gameManager;

        private void Awake() {
            
            _gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        }

        public void IncrementFloatState(int value) {
            if (_gameManager.GetState(stateType).StateData == null) {
                _gameManager.SetStateData(stateType, new FloatData(0, 5f));
            }
            _gameManager.UpdateState(stateType, ((float)_gameManager.GetState(stateType).StateData.GetData()) + value);
        }

        public void FinishState() {
        
            _gameManager.FinishState(stateType);
        }
        
    }
}
