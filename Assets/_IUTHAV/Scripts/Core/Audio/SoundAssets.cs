using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Core.Audio
{
    public class SoundAssets : MonoBehaviour
    {
        private static SoundAssets _instance; 

        public static SoundAssets instance
        {
            get{
                if (_instance == null) _instance = (Instantiate(Resources.Load("SoundAssets")) as GameObject)?.GetComponent<SoundAssets>();
                return _instance;
            }
        }

        private void Awake()
        {
            SoundManager.SetupAudioManager(null);
            _instance = this; 
        }

        public SoundAudioClip[] soundAudioClips; 
        [System.Serializable]
        public class SoundAudioClip
        {
            public SoundManager.SoundType soundType;
            [FormerlySerializedAs("audioClip")] public AudioClip[] audioClips;
            [SerializeField] public float volume = 1;
            
            public void Validate()
            {
                if (volume == 0) volume = 1; // Ensure new entries default to 1
            }
        }
    }
}