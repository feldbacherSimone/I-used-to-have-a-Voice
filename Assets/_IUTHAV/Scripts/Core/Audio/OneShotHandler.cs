using System;
using System.Collections;
using UnityEngine;

namespace _IUTHAV.Scripts.Core.Audio
{
    public class OneShotHandler : MonoBehaviour
    {
        [SerializeField] private bool available = true;
        private AudioSource _audioSource; 

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public bool IsAvailable()
        {
            return available; 
        }

        public AudioSource getAudioSource()
        {
            return _audioSource;
        }

        public void setAudioSource(AudioSource audioSource)
        {
            _audioSource = audioSource; 
        }
        public void PlayOneShotClip(AudioClip clip)
        {
            available = false;
            _audioSource.clip = clip;
            StartCoroutine(playOneShotClipInternal());

        }

        private IEnumerator playOneShotClipInternal()
        {
            _audioSource.Play();
            while (_audioSource.isPlaying)
            {
                yield return null; 
            }
            available = true;
            StopCoroutine(playOneShotClipInternal());
        }
    }
}