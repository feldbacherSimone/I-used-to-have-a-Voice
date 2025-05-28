using System;
using UnityEngine;

namespace _IUTHAV.Scripts.Core.Audio {
    public class SoundController : MonoBehaviour {

        [SerializeField] private bool isDebug;
        
        public void PlaySFXSound(string soundType) { SoundManager.PlaySound(StringToSoundType(soundType), SoundManager.Mixer.SFX); }
        
        public void PlayAmbientSound(string soundType) { SoundManager.PlaySound(StringToSoundType(soundType), SoundManager.Mixer.Ambient); }
        
        public void PlayDialogueSound(string soundType) { SoundManager.PlaySound(StringToSoundType(soundType), SoundManager.Mixer.Dialogue); }
        
        public void PlayUISound(string soundType) { SoundManager.PlaySound(StringToSoundType(soundType), SoundManager.Mixer.UI); }
        
        public void PlayMusic(string soundType) { SoundManager.PlaySound(StringToSoundType(soundType), SoundManager.Mixer.Music); }

        private SoundManager.SoundType StringToSoundType(string name) {
            if (Enum.TryParse(name, out SoundManager.SoundType type)) {
                return type;
            }
            else {
                if (isDebug) Debug.LogWarning($"Sound {name} has no enum entry");
                return 0;
            }
        }
    }
}