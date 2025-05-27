using System;
using System.Collections;
using System.Collections.Generic;
using _IUTHAV.Scripts.Core.Audio;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuToggle : MonoBehaviour
{

    [SerializeField] private MenuPair[] _menuPairs;
    [SerializeField] private SoundManager.SoundType soundType; 

    
    // Start is called before the first frame update
    void Awake()
    {
        foreach (MenuPair menuPair in _menuPairs)
        {
            menuPair.button.onClick.AddListener(() =>
            {
                SoundManager.PlaySound(SoundManager.SoundType.UIClick, SoundManager.Mixer.SFX);
                menuPair.visual.SetActive(true);
            });
            menuPair.cancelButton.onClick.AddListener(() =>
            {
                SoundManager.PlaySound(SoundManager.SoundType.UIClick, SoundManager.Mixer.SFX);
                menuPair.visual.SetActive((false));
            });
            menuPair.visual.SetActive(menuPair.startVisibility);
        }
    }
    
    
}

[Serializable]
public struct MenuPair
{
    public Button button;
    public Button cancelButton;
    public GameObject visual;
    public bool startVisibility; 
}