using System;
using UnityEngine;

namespace _IUTHAV.Scripts.Lerpables {
    [Serializable]
    public class LerpToonShaderParameter : LerpParameter<Material> {
        
        [SerializeField] private Material toonMaterial;
        
        private const string RimColorName = "_RimColor";
        private const string AmbientColorName = "_AmbientColor";

        public override void SetLerpValue(float targetValue) {

            if (_currentParameterValue == null) {
                _currentParameterValue = new Material(toonMaterial.shader);
            }
        
            _currentParameterValue.color = Vector4.Lerp(minLerpParam.color, maxLerpParam.color, targetValue);
            _currentParameterValue.SetColor(RimColorName, Vector4.Lerp(minLerpParam.GetColor(RimColorName), maxLerpParam.GetColor(RimColorName), targetValue));
            _currentParameterValue.SetColor(AmbientColorName, Vector4.Lerp(minLerpParam.GetColor(AmbientColorName), maxLerpParam.GetColor(AmbientColorName),
                targetValue));
            
            toonMaterial.color = _currentParameterValue.color;
            toonMaterial.SetColor(RimColorName, _currentParameterValue.GetColor(RimColorName));
            toonMaterial.SetColor(AmbientColorName, _currentParameterValue.GetColor(AmbientColorName));
        }
        
    }
}