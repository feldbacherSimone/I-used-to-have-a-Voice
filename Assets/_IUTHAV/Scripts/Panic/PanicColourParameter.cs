using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.Panic {

    [Serializable]
    public class PanicColorParameter : PanicParameter<Color> {
    
        [CanBeNull] [SerializeField] private Image image;
        [SerializeField] private Light lightSource;
        [CanBeNull] [SerializeField] private Material material;
        
        //Mask to make checking for null faster
        private ushort _mask = 0b000000;

        public void Awake() {
            Configure();
        }

        public void Configure() {

            if (image != null) _mask |= 0b000001;
            if (lightSource != null) _mask |= 0b000010;
            if (material != null) _mask |= 0b000100;

            _currentParameterValue = minPanicParameter;
            SetDesiredParameter();
        }

        public override void SetBaseParameter() {
            if ((_mask & 0b000001) == 0b000001) {
                _baseParameterValue = image.color;
                return;
            }

            if ((_mask & 0b000010) == 0b000010) {
                _baseParameterValue = lightSource.color;
                return;
            }

            if ((_mask & 0b000100) == 0b000100) {
                _baseParameterValue = material.color;
                return;
            }
        }
        
        public override void SetTargetParameter(float targetValue) {
            _targetParameterValue = Vector4.Lerp(minPanicParameter, maxPanicParameter, targetValue);
        }

        public override void SetDesiredParameter() {

            if ((_mask & 0b000001) == 0b000001) image.color = _currentParameterValue;
            if ((_mask & 0b000001) == 0b000010) lightSource.color = _currentParameterValue;
            if ((_mask & 0b000100) == 0b000100) material.color = _currentParameterValue;
            
        }

        public override void LerpByPanicValue(float targetValue) {
            _currentParameterValue = Vector4.Lerp(_baseParameterValue, _targetParameterValue, targetValue);
        }
    }


}
