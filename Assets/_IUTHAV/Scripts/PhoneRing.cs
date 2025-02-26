using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneRing : MonoBehaviour
{
    [SerializeField] private Animation _animation; 
    public void RingPhone(float delay)
    {
        StartCoroutine(RingPhoneRoutine(delay));
    }

    IEnumerator RingPhoneRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        _animation.Play();
        SoundManager.PlaySound(SoundManager.Sound.PhoneRing, SoundManager.Mixer.SFX);
    }
    
}
