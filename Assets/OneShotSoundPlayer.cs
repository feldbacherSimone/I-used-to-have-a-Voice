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
    [SerializeField] private float delay; 
    public void PlayOneShot()
    {
        StartCoroutine(WaitAndPlay());
    }

    IEnumerator WaitAndPlay()
    {
        yield return new WaitForSeconds(delay);
        SoundManager.PlaySound(sound, mixer);
    }
}
