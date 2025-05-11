using System.Collections;
using System.Collections.Generic;
using _IUTHAV.Scripts.Core.Audio;
using UnityEngine;

public class SoundManagerTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.PlaySound(SoundManager.SoundType.PhoneRing, SoundManager.Mixer.SFX);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
