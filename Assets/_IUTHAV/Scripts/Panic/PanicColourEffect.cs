using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.Panic {
    public class PanicColourEffect : MonoBehaviour, IChangeFloat {

        [SerializeField] private ColourParameter[] colorParamters;

        public void Awake() {

            foreach (var col in colorParamters) {
            
                col.Configure();
                
            }

            StartCoroutine(WaitAndChangeColour());

        }

        public void ChangeFloat(float deltaTime, float targetValue) {

            StartCoroutine(ChangeColour(deltaTime, targetValue));

        }

        private IEnumerator WaitAndChangeColour() {
            StartCoroutine(ChangeColour(2f, 0.8f));
            yield return new WaitForSeconds(4);
            StartCoroutine(ChangeColour(2f, 0));
        }

        private IEnumerator ChangeColour(float deltaTime, float targetValue) {
            
            float t = 0;

            foreach (var col in colorParamters) {
                col.SetBaseColor();
                col.SetTargetColor(targetValue);
            }
            
            while (t < deltaTime) {
                
                foreach (var col in colorParamters) {

                    Vector4 newVector4 = Vector4.Lerp(col.BaseColor, col.TargetColor, t / deltaTime);

                    col.ChangeColor(newVector4);
                    
                    Debug.Log(col.CurrentColor + " | " + t / deltaTime);
                }

                t += Time.deltaTime;
                yield return null;
            }
        }
        
        [Serializable]
        public class ColourParameter {
            [CanBeNull] [SerializeField] private Image image;
            [CanBeNull] [SerializeField] private Light light;
            [CanBeNull] [SerializeField] private Material material;
            [SerializeField] private Color minPanicColor;
            [SerializeField] private Color maxPanicColor;
            private Color _currentColor;
            public Color CurrentColor => _currentColor;
            private Color _baseColor;
            public Color BaseColor => _baseColor;
            private Color _targetColor;
            public Color TargetColor => _targetColor;

            private byte _mask = 0b0000;

            public void Configure() {

                if (image != null) _mask |= 0b0001;
                if (light != null) _mask |= 0b0010;
                if (material != null) _mask |= 0b0100;
                
                ChangeColor(_baseColor);

            }

            public void ChangeColor(Color color) {

                _currentColor = color;
                
                if ((_mask & 0b0001) == 0b0001) image.color = color;
                if ((_mask & 0b0010) == 0b0010) light.color = color;
                if ((_mask & 0b0100) == 0b0100) material.color = color;

            }

            public void SetTargetColor(float targetValue) {
                Vector4.Lerp(minPanicColor, maxPanicColor, targetValue);
            }

            public void SetBaseColor() {
                if ((_mask & 0b0001) == 0b0001) {
                    _currentColor = image.color;
                    return;
                }

                if ((_mask & 0b0010) == 0b0010) {
                    _currentColor = light.color;
                    return;
                }

                if ((_mask & 0b0100) == 0b0100) {
                    _currentColor = material.color;
                    return;
                }
            }
        }
    }

    
}
