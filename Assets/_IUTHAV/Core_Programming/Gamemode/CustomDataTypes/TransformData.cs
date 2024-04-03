using UnityEngine;

namespace _IUTHAV.Core_Programming.Gamemode.CustomDataTypes {
    public class TransformData : MonoBehaviour {

        [SerializeField] private StateType stateType;
        [SerializeField] private Transform transformData;
        [SerializeField] private Vector3 targetTransform;
        [SerializeField] private VectorType vectorType;
        [SerializeField] private CompareType compareType;

        private void Start() {
        
            GameManager gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

            float floatData = transformData.position.x;
            float targetData = targetTransform.x;

            switch (vectorType) {
                case VectorType.x:
                    floatData = transformData.position.x;
                    targetData = targetTransform.x;
                    break;
                case VectorType.y:
                    floatData = transformData.position.y;
                    targetData = targetTransform.y;
                    break;
                case VectorType.z:
                    floatData = transformData.position.z;
                    targetData = targetTransform.z;
                    break;
            }
            
            gameManager.SetStateData(stateType, new FloatData(
                floatData, targetData, compareType
                
            ));
                
        }
    }
}
