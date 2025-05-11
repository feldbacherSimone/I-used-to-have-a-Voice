using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Core.Audio
{
    
    //TODO: Deprecate this
    public class OneShotSoundPlayer : MonoBehaviour
    {
    
        [SerializeField] private SoundManager.Mixer mixer; 
        [SerializeField] private SoundManager.SoundType soundType; 
        [SerializeField] private float delay; 
        public void PlayOneShot()
        {
            StartCoroutine(WaitAndPlay());
        }

        IEnumerator WaitAndPlay()
        {
            yield return new WaitForSeconds(delay);
            SoundManager.PlaySound(soundType, mixer);
        }
    }
}
