using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _IUTHAV.Scripts.Core.Audio
{
    public class CharacterVoice : MonoBehaviour
    {
        [SerializeField] private String characterName;
        [SerializeField] private float soundFrequency;
        [SerializeField] private float volume; 
        [SerializeField] private AudioClip[] voiceClips;
        
        private float timeSinceLastSound;
        private float lastTime; 

        private void Awake()
        {
            lastTime = Time.unscaledTime;
        }

        public AudioClip TryGetClip()
        {
            timeSinceLastSound = Time.unscaledTime - lastTime;
            print($"Unscaled Time: {Time.unscaledTime}, Time since last sound: {timeSinceLastSound}" );
            if (timeSinceLastSound > 1/soundFrequency)
            {
                lastTime = Time.unscaledTime;
                return voiceClips[Mathf.RoundToInt(Random.Range(0, voiceClips.Length))];
            }
            return null; 
        }

        public String GetCharacterName()
        {
            return characterName; 
        }
    }
}