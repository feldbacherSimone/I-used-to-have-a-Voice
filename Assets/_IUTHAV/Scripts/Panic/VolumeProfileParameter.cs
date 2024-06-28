using UnityEngine;
using UnityEngine.Rendering;

namespace _IUTHAV.Scripts.Panic {
    public class VolumeProfileParameter : PanicParameter<float> {

        [SerializeField] private Volume profile;
        
        public override void SetDesiredParameter() {
            profile.weight = _currentParameterValue;
        }

        public override void SetBaseParameter() {
            _baseParameterValue = profile.weight;
        }

        public override void SetTargetParameter(float targetValue) {
            _targetParameterValue = Mathf.Lerp(minPanicParameter, maxPanicParameter, targetValue);
        }

        public override void LerpByPanicValue(float targetValue) {
            _currentParameterValue = Mathf.Lerp(_baseParameterValue, _targetParameterValue, targetValue);
        }
    }
}
