using System;
using System.Collections;
using System.Collections.Generic;
using _IUTHAV.Scripts.Core.Audio;
using UnityEngine;

public class SFXUtils : MonoBehaviour
{
    private AudioClipRandomizer _audioClipRandomizer;

    #region UnityFunctions

    private void Start()
    {
        _audioClipRandomizer = GetComponent<AudioClipRandomizer>();
        if(!_audioClipRandomizer) Debug.LogError($"No AudioClipRandomizer found!", gameObject);
    }
    #endregion

    #region PublicFunctions

    

    #endregion
}
