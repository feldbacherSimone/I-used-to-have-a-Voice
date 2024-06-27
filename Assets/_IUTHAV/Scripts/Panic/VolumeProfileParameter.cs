using System;
using Meryuhi.Rendering;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines.Interpolators;

namespace _IUTHAV.Scripts.Panic {
    public class VolumeProfileParameter : PanicParameter<VolumeProfile> {

        [SerializeField] private VolumeProfile profile;
        [SerializeField] private VolumeProfile bufferProfileBase;
        [SerializeField] private VolumeProfile bufferProfileCurrent;
        [SerializeField] private VolumeProfile bufferProfileTarget;

        private void Awake() {

            _baseParameterValue = bufferProfileBase;
            _currentParameterValue = bufferProfileCurrent;
            _targetParameterValue = bufferProfileTarget;
        
            CopyComponentValues(_currentParameterValue, minPanicParameter);
            SetDesiredParameter();
            CopyComponentValues(_targetParameterValue, minPanicParameter);
        }


        public override void SetDesiredParameter() {
        
            CopyComponentValues(profile, _currentParameterValue);
            
        }

        public override void SetBaseParameter() {
            
            CopyComponentValues(_baseParameterValue, profile);
            
        }

        public override void SetTargetParameter(float targetValue) {
            
            LerpComponentValues(_targetParameterValue, minPanicParameter, maxPanicParameter, targetValue);
        }

        public override void LerpByPanicValue(float targetValue) {
            LerpComponentValues(_currentParameterValue, _baseParameterValue, _targetParameterValue, targetValue);
        }

        private void CopyComponentValues(VolumeProfile profileA, VolumeProfile profileB) {
            
            if (profileA.TryGet(out SplitToning aToning)) {
            
                profileB.TryGet(out SplitToning bToning);

                aToning.highlights.SetValue(bToning.highlights);
                aToning.shadows.SetValue(bToning.shadows);
                aToning.balance.SetValue(bToning.balance);

            }
            
            if (profileA.TryGet(out ColorAdjustments adj)) {
                
                profileB.TryGet(out ColorAdjustments bAdj);

                adj.postExposure.SetValue(bAdj.postExposure);
                adj.contrast.SetValue(bAdj.contrast);
                adj.saturation.SetValue(bAdj.saturation);
                adj.hueShift.SetValue(bAdj.hueShift);
                adj.colorFilter.SetValue(bAdj.colorFilter);
//
            }
            
            if (profileA.TryGet(out FullScreenFog fog)) {
            
                profileB.TryGet(out FullScreenFog bFog);

                fog.endHeight.SetValue(bFog.endHeight);
                fog.endLine.SetValue(bFog.endLine);
                fog.density.SetValue(bFog.density);
                fog.intensity.SetValue(bFog.intensity);
                fog.startHeight.SetValue(bFog.startHeight);
                fog.startLine.SetValue(bFog.startLine);
                fog.color.SetValue(bFog.color);
//
            }
            
        }
        
        private void LerpComponentValues(VolumeProfile affectedProfile, VolumeProfile profileA, VolumeProfile profileB, float t) {

            VolumeParameter colorParameter = new ColorParameter(Vector4.zero);
            VolumeParameter floatParam = new FloatParameter(0f);
            
            if (affectedProfile.TryGet(out SplitToning toning)) {
                
                profileA.TryGet(out SplitToning baseToning);
                profileB.TryGet(out SplitToning targetToning);

                colorParameter =
                    new ColorParameter(Vector4.Lerp(baseToning.highlights.value, targetToning.highlights.value, t));
                
                toning.highlights.SetValue(colorParameter);
                
                colorParameter =
                    new ColorParameter(Vector4.Lerp(baseToning.shadows.value, targetToning.shadows.value, t));
                
                toning.shadows.SetValue(colorParameter);

                floatParam =
                    new FloatParameter(Mathf.Lerp(baseToning.balance.value, targetToning.balance.value, t));
                toning.balance.SetValue(floatParam);

            }
            
            if (affectedProfile.TryGet(out ColorAdjustments adj)) {
                
                profileA.TryGet(out ColorAdjustments baseAdj);
                profileB.TryGet(out ColorAdjustments targetAdj);
                
                floatParam = new FloatParameter(Mathf.Lerp(baseAdj.postExposure.value, targetAdj.postExposure.value, t));
                adj.postExposure.SetValue(floatParam);

                floatParam =
                    new FloatParameter(Mathf.Lerp(baseAdj.contrast.value, targetAdj.contrast.value, t));
                adj.contrast.SetValue(floatParam);

                floatParam =
                    new FloatParameter(Mathf.Lerp(baseAdj.saturation.value, targetAdj.saturation.value, t));
                adj.saturation.SetValue(floatParam);

                floatParam =
                    new FloatParameter(Mathf.Lerp(baseAdj.hueShift.value, targetAdj.hueShift.value, t));
                adj.hueShift.SetValue(floatParam);

                colorParameter =
                    new ColorParameter(Vector4.Lerp(baseAdj.colorFilter.value, targetAdj.colorFilter.value, t));
                adj.colorFilter.SetValue(colorParameter);

            }
            
            if (affectedProfile.TryGet(out FullScreenFog fog)) {
                
                profileA.TryGet(out FullScreenFog baseFog);
                profileB.TryGet(out FullScreenFog targetFog);

                colorParameter =
                    new ColorParameter(Vector4.Lerp(baseFog.color.value, targetFog.color.value, t));
                fog.color.SetValue(colorParameter);
            }
            
        }
    }
}