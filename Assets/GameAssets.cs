using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance; 

    public static GameAssets instance
    {
        get{
            if (_instance == null) _instance = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _instance;
        }
    }

    private void Awake()
    {
        SoundManager.LoadMixer();
        GameAssets._instance = this; 
    }

    public SoundAudioClip[] soundAudioClips; 
    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip[] audioClip;
        public float volume = 1; 
    }
}