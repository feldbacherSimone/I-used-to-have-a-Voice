using UnityEngine;

namespace _IUTHAV.Scripts.Core.Audio {
    public class AudioEffects : MonoBehaviour {

        [SerializeField] private float fxLerpTime;

        public void FadeInLowPass(float floor) {

            if (AudioFXController.Lowpasscutofffreq < floor) {
                StartCoroutine(AudioFXController.FadeIn(
                AudioFXType.Lowpasscutofffreq,
                fxLerpTime,
                floor));
            }
            else {
                StartCoroutine(AudioFXController.FadeOut(
                AudioFXType.Lowpasscutofffreq,
                fxLerpTime,
                floor));
            }
        }

    }
}