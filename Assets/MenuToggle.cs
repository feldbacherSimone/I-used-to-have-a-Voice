using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuToggle : MonoBehaviour
{

    [SerializeField] private MenuPair[] _menuPairs;

    
    // Start is called before the first frame update
    void Awake()
    {
        foreach (MenuPair menuPair in _menuPairs)
        {
            menuPair.button.onClick.AddListener(() => menuPair.visual.SetActive((!menuPair.visual.activeSelf)));
            menuPair.visual.SetActive(menuPair.startVisibility);
        }
    }
    
    
}

[Serializable]
public struct MenuPair
{
    public Button button;
    public GameObject visual;
    public bool startVisibility; 
}