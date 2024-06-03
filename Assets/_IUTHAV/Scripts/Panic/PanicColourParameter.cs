using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.Panic {

    [Serializable]
    public class PanicColorParameter : PanicParameter<Color> {
    
        [CanBeNull] [SerializeField] private Image image;
        [CanBeNull] [SerializeField] private Light light;
        [CanBeNull] [SerializeField] private Material material;

        private byte _mask = 0b0000;

        public void Awake() {
            Configure();
        }

        public void Configure() {

            if (image != null) _mask |= 0b0001;
            if (light != null) _mask |= 0b0010;
            if (material != null) _mask |= 0b0100;

            _currentParameterValue = minPanicParameter;
            ChangeComponentParameter();
        }

        public override void SetBaseParameter() {
            if ((_mask & 0b0001) == 0b0001) {
                _baseParameterValue = image.color;
                return;
            }

            if ((_mask & 0b0010) == 0b0010) {
                _baseParameterValue = light.color;
                return;
            }

            if ((_mask & 0b0100) == 0b0100) {
                _baseParameterValue = material.color;
                return;
            }
        }
        
        public override void SetTargetParameter(float targetValue) {
            _targetParameterValue = Vector4.Lerp(minPanicParameter, maxPanicParameter, targetValue);
        }

        public override void ChangeComponentParameter() {

            if ((_mask & 0b0001) == 0b0001) image.color = _currentParameterValue;
            if ((_mask & 0b0001) == 0b0010) light.color = _currentParameterValue;
            if ((_mask & 0b0100) == 0b0100) material.color = _currentParameterValue;
            
        }

        public override void LerpByPanicDelta(float targetValue) {
            _currentParameterValue = Vector4.Lerp(_baseParameterValue, _targetParameterValue, targetValue);
        }
    }


}
