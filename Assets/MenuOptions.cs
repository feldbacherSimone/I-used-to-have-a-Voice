using System;
using System.Collections;
using System.Collections.Generic;
using _IUTHAV.Scripts.Core.Audio;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro; 

public class MenuOptions : MonoBehaviour
{

    [SerializeField] private Slider s_musicVolume; 
    [SerializeField] private Slider s_sfxVolume;

    private static string V_DIALOGUE = "v_dialogue";
    private static string V_MUSIC = "v_music";
    private static string V_SFX = "v_sfx";
    private static string V_AMBIENT = "v_ambient";
    
    private AudioMixer mixer;

    [SerializeField] private TMP_Dropdown dd_languageSelect;

    [SerializeField] private float soundOffset = 0.5f;
    private float lastTime; 
    IEnumerator Start()
    {
        lastTime = Time.time;
        
        //Setup language dropdown 
        yield return LocalizationSettings.InitializationOperation;

        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale) selected = i; 
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }

        dd_languageSelect.options = options; 
        dd_languageSelect.value = selected; 
        dd_languageSelect.onValueChanged.AddListener(LanguageSelected);
        
        //Setup volume slider
        mixer = Resources.Load<AudioMixer>("Mixer");
        if(mixer == null) Debug.LogError($"could not find audio mixer in resources");
        
        s_musicVolume.onValueChanged.AddListener(delegate(float sliderValue) { onVolumeChanged(new []{V_MUSIC}, sliderValue); });
        s_sfxVolume.onValueChanged.AddListener( delegate(float sliderValue) {  onVolumeChanged(new []{V_SFX, V_AMBIENT}, sliderValue);});
    }



    private void LanguageSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    private void onVolumeChanged(string[] channels, float sliderValue)
    {
        foreach (var channel in channels)
        {
            mixer.SetFloat(channel, Mathf.Log10(sliderValue) * 20);
          
        }
        
        if (Time.time - lastTime > soundOffset)
        {
            SoundManager.PlaySound(SoundManager.SoundType.UIClick, SoundManager.Mixer.SFX);
            lastTime = Time.time;
        }
    }
}
