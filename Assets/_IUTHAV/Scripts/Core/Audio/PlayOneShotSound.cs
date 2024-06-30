using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _IUTHAV.Scripts.Core.Audio
{
    public class PlayOneShotSound : MonoBehaviour
    {
        public void PlayOneShot(AudioClip clip)
        {
            if(!clip) return;
            GameObject oneShotObject = new GameObject();
            AudioSource oneShotSource = oneShotObject.AddComponent<AudioSource>();

            oneShotSource.clip = clip;
            oneShotSource.Play();
            StartCoroutine(deleteSourceAfterPlay(clip.length, oneShotObject));
        }

        IEnumerator deleteSourceAfterPlay(float seconds, GameObject sourceObject)
        {
            yield return new WaitForSeconds(seconds);
            Destroy(sourceObject);
        }
    }
    
    
    
}