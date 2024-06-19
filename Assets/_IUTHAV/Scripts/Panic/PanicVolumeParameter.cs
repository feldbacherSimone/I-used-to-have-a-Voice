using UnityEngine;

namespace _IUTHAV.Scripts.Panic {
    public class PanicVolumeParameter : PanicParameter<float> {

        [SerializeField] private AudioSource source;
        public override void SetDesiredParameter() {
            source.volume = _currentParameterValue;
        }

        public override void SetBaseParameter() {
            _baseParameterValue = source.volume;
        }

        public override void SetTargetParameter(float targetValue) {
            _targetParameterValue = Mathf.Lerp(minPanicParameter, maxPanicParameter, targetValue);
        }

        public override void LerpByPanicValue(float targetValue) {
            _currentParameterValue = Mathf.Lerp(_baseParameterValue, _targetParameterValue, targetValue);
        }
    }
}