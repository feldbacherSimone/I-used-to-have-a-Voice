using System;
using UnityEngine;

namespace _IUTHAV.Scripts.Lerpables {

    [Serializable]
    public class LerpTransformParameter : LerpParameter<Transform> {

        [SerializeField] private Transform targetTransform;
        [SerializeField] private bool translatePosition;
        [SerializeField] private bool translateRotation;
        [SerializeField] private bool translateScale;
        
        public override void SetLerpValue(float targetValue) {
            if (translatePosition) _currentParameterValue.localPosition = Vector3.Lerp(minLerpParam.localPosition, maxLerpParam.localPosition, targetValue);
            if (translateRotation) _currentParameterValue.localRotation = Quaternion.Euler(Vector3.Lerp(minLerpParam.localRotation.eulerAngles, maxLerpParam.localRotation.eulerAngles, targetValue));
            if (translateScale) _currentParameterValue.localScale = Vector3.Lerp(minLerpParam.localScale, maxLerpParam.localScale, targetValue);
            
            if (translatePosition) targetTransform.localPosition = _currentParameterValue.localPosition;
            if (translateRotation) targetTransform.localRotation = _currentParameterValue.localRotation;
            if (translateScale) targetTransform.localScale = _currentParameterValue.localScale;
        }
    }
}