using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OneShotSoundPlayer : MonoBehaviour
{
    private void Start()
    {
        SoundManager.LoadMixer();
    }

    [SerializeField] private SoundManager.Mixer mixer; 
    [SerializeField] private SoundManager.Sound sound; 
    public void PlayOneShot()
    {
        SoundManager.PlaySound(sound, mixer);
    }
}
