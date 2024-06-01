using UnityEngine;

namespace _IUTHAV.Scripts.Core.Audio {
    public class AudioOptionsController : MonoBehaviour {
        
        public static float MasterVolume { get; private set; }
        public static float MusicVolume { get; private set; }
        public static float DialogueVolume { get; private set; }
        public static float SFXVolume { get; private set; }
        public static float AmbientVolume { get; private set; }

        private void Start() {
            GetVolumePrefs();
        }

        public void SetMasterVolume(float value) {
            MasterVolume = value;
            AudioController.UpdateMixerVolume();
        }

        public void SetMusicVolume(float value) {
            MusicVolume = value;
            AudioController.UpdateMixerVolume();
        }
        
        public void SetDialogueVolume(float value) {
            DialogueVolume = value;
            AudioController.UpdateMixerVolume();
        }
        
        public void SetSFXVolume(float value) {
            SFXVolume = value;
            AudioController.UpdateMixerVolume();
        }
        
        public void SetAmbientVolume(float value) {
            AmbientVolume = value;
            AudioController.UpdateMixerVolume();
        }

        public static void SetVolumePrefs() {
            //TODO: Change this with playerPrefs
            /*
            DataController.SetVolumePrefs(new [] {
                MasterVolume,
                MusicVolume,
                DialogueVolume,
                SFXVolume,
                AmbientVolume
            });
            */
        }
        
        private static void GetVolumePrefs() {
        
            if (PlayerPrefs.HasKey(AudioGroupType.Master.ToString())) {
                MasterVolume += PlayerPrefs.GetFloat(AudioGroupType.Master.ToString());
            } 
            

            if (PlayerPrefs.HasKey(AudioGroupType.Music.ToString())) {
                MusicVolume += PlayerPrefs.GetFloat(AudioGroupType.Music.ToString());
            } 
            

            if (PlayerPrefs.HasKey(AudioGroupType.Dialogue.ToString())) {
                DialogueVolume += PlayerPrefs.GetFloat(AudioGroupType.Dialogue.ToString());
            }
            

            if (PlayerPrefs.HasKey(AudioGroupType.SFX.ToString())) {
                SFXVolume += PlayerPrefs.GetFloat(AudioGroupType.SFX.ToString());
            }
            

            if (PlayerPrefs.HasKey(AudioGroupType.Ambient.ToString())) {
                AmbientVolume += PlayerPrefs.GetFloat(AudioGroupType.Ambient.ToString());
            }
            
            AudioController.UpdateMixerVolume();
        }
    }
}
