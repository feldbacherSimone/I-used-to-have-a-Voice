using System;
using JetBrains.Annotations;
using UnityEngine;

namespace _IUTHAV.Scripts.Panic {

    [Serializable]
    public class PanicVectorParameter : PanicParameter<Transform> {

        [CanBeNull] [SerializeField] private Transform targetTransform;
        [SerializeField] private bool translatePosition;
        [SerializeField] private bool translateRotation;
        [SerializeField] private bool translateScale;

        public void Awake() {

            if (targetTransform == null) {
                Debug.LogWarning("No targetTransform has been assigned, disabling this script");
                this.gameObject.SetActive(false);
            }
            _currentParameterValue = minPanicParameter;
            SetDesiredParameter();
        }
        public override void SetDesiredParameter() {
            if (translatePosition) targetTransform.localPosition = _currentParameterValue.localPosition;
            if (translateRotation) targetTransform.localRotation = _currentParameterValue.localRotation;
            if (translateScale) targetTransform.localScale = _currentParameterValue.localScale;
        }

        public override void SetBaseParameter() {
            if (translatePosition) _baseParameterValue.localPosition = targetTransform.localPosition;
            if (translateRotation) _baseParameterValue.localRotation = targetTransform.localRotation;
            if (translateScale) _baseParameterValue.localScale = targetTransform.localScale;
        }

        public override void SetTargetParameter(float targetValue) {
            if (translatePosition) _targetParameterValue.localPosition = Vector3.Lerp(minPanicParameter.localPosition, maxPanicParameter.localPosition, targetValue);
            if (translateRotation) _targetParameterValue.localRotation = Quaternion.Euler(Vector3.Lerp(minPanicParameter.localRotation.eulerAngles, maxPanicParameter.localRotation.eulerAngles, targetValue));
            if (translateScale) _targetParameterValue.localScale = Vector3.Lerp(minPanicParameter.localScale, maxPanicParameter.localScale, targetValue);
        }

        public override void LerpByPanicValue(float targetValue) {
            if (translatePosition) _currentParameterValue.localPosition = Vector3.Lerp(minPanicParameter.localPosition, maxPanicParameter.localPosition, targetValue);
            if (translateRotation) _currentParameterValue.localRotation = Quaternion.Euler(Vector3.Lerp(minPanicParameter.localRotation.eulerAngles, maxPanicParameter.localRotation.eulerAngles, targetValue));
            if (translateScale) _currentParameterValue.localScale = Vector3.Lerp(minPanicParameter.localScale, maxPanicParameter.localScale, targetValue);
        }
    }
}