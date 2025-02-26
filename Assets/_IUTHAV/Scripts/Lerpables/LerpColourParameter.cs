using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.Lerpables {

    [Serializable]
    public class LerpColourParameter : LerpParameter<Color> {
        
        [EnumFlags]
        [SerializeField] 
        private ColorParameterType parameterType;
        
        [ShowIf("parameterType", ColorParameterType.Image)]
        [SerializeField] 
        private Image image;
        
        [ShowIf("parameterType", ColorParameterType.Light)]
        [SerializeField] 
        private Light lightSource;
        
        [ShowIf("parameterType", ColorParameterType.Material)]
        [SerializeField] 
        private Material material;

        private void SetDesiredParameter() {

            switch (parameterType) {
                case ColorParameterType.Image:
                    image.color = _currentParameterValue;
                    break;
                case ColorParameterType.Light:
                    lightSource.color = _currentParameterValue;
                    break;
                case ColorParameterType.Material:
                    material.color = _currentParameterValue;
                    break;
            }
            
        }

        public override void SetLerpValue(float targetValue) {
            _currentParameterValue = Vector4.Lerp(minLerpParam, maxLerpParam, targetValue);
            SetDesiredParameter();
        }
    }

    public enum ColorParameterType {
        Image,
        Light,
        Material
    }


}
