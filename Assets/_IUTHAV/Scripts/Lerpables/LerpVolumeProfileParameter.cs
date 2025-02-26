using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace _IUTHAV.Scripts.Lerpables {
    [Serializable]
    public class LerpVolumeProfileParameter : LerpParameter<float> {

        [SerializeField] private Volume profile;
        
        public override void SetLerpValue(float targetValue) {
            _currentParameterValue = Mathf.Lerp(minLerpParam, maxLerpParam, targetValue);
            profile.weight = _currentParameterValue;
        }
    }
}
