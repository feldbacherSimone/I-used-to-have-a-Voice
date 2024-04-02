using UnityEngine;

namespace _IUTHAV.Core_Programming.Gamemode {
    public class UpdateState : MonoBehaviour {

        [SerializeField] private StateType stateType;

        private GameManager _gameManager;

        private void Awake() {
            _gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        }

        public void FinishState() {
        
            _gameManager.GetState(stateType).Finish();
            
        }
        
    }
}
