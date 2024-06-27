using System.Collections;
using UnityEngine;

namespace _IUTHAV.Scripts.Core.Audio {
    public static class AudioFXController {

        public static float Lowpasscutofffreq { get; private set; }
        public static float Lowpassresonance { get; private set; }
        public static float Masterechodelay { get; private set; }
        public static float Masterechodecay { get; private set; }

        private static float _currentFloor = 20f;

        public static IEnumerator FadeIn(AudioFXType fxType, float fadeTime, float floor = 20f) {
            float currentTime = 0.01f;
            _currentFloor = floor;
            
            while (currentTime < fadeTime) {

                switch (fxType) {
                    case AudioFXType.Lowpasscutofffreq:
                        Lowpasscutofffreq = 22000 - (22000 * currentTime / fadeTime) + floor;
                        break;
                    case AudioFXType.Lowpassresonance:
                        Lowpassresonance = currentTime / fadeTime;
                        break;
                    case AudioFXType.Masterechodelay:
                        Masterechodelay = currentTime / fadeTime;
                        break;
                    case AudioFXType.Masterechodecay:
                        Masterechodecay = currentTime / fadeTime;
                        break;
                }
                
                AudioController.UpdateMixerFX();
                currentTime += Time.deltaTime;
                yield return null;
            }
            
            yield return null;
        }
        
        
        public static IEnumerator FadeOut(AudioFXType fxType, float fadeTime, float ceiling = 22000) {
            float currentTime = 0.1f;

            while (currentTime < fadeTime) {
                switch (fxType) {
                    case AudioFXType.Lowpasscutofffreq:
                        Lowpasscutofffreq = _currentFloor + 22000 * currentTime / fadeTime - (ceiling-22000);
                        break;
                    case AudioFXType.Lowpassresonance:
                        Lowpassresonance = currentTime / fadeTime;
                        break;
                    case AudioFXType.Masterechodelay:
                        Masterechodelay = currentTime / fadeTime;
                        break;
                    case AudioFXType.Masterechodecay:
                        Masterechodecay = currentTime / fadeTime;
                        break;
                }
                
                AudioController.UpdateMixerFX();
                currentTime += 0.1f + Time.deltaTime;
                yield return new WaitForSeconds(0.1f + Time.deltaTime);
            }

            _currentFloor = 20f;
            yield return null;
        }
    }
}