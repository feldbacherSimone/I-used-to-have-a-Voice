using System;
using System.Collections;
using System.Collections.Generic;
using _IUTHAV.Scripts.Core.Audio;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
   [SerializeField] private CharacterVoice[] voices;
   [SerializeField] private AudioSource _audioSource;
   [SerializeField] private bool isDebug; 

   private Dictionary<String, CharacterVoice> voiceList = new Dictionary<string, CharacterVoice>();
   private string currentCharacter; 
   private void Awake()
   {
      foreach (var voice in voices)
      {
         voiceList.Add(voice.GetCharacterName(), voice);
      }
   }

   public void SetCharacter(string name)
   {
      if (!voiceList.ContainsKey(name))
      {
         Debug.LogError($"No Character Voice: {name} found");
         return;
      }

      currentCharacter = name;
      DebugLog($"Current Character: {currentCharacter}");
   }

   public void RequestVoiceSound()
   {
      if(currentCharacter == null) return;

      AudioClip currentClip = voiceList[currentCharacter].TryGetClip();
      PlayOneShot(currentClip);
   }

   private void DebugLog(string message)
   {
      if (isDebug)
      {
         Debug.Log(message);
      }
   }

   private void PlayOneShot(AudioClip clip)
   {
      if(!clip) return;
      GameObject oneShotObject = new GameObject("CharacterVoice");
      AudioSource oneShotSource = oneShotObject.AddComponent<AudioSource>();

      oneShotSource.clip = clip;
      oneShotSource.Play();
      StartCoroutine(deleteSourceAfterPlay(clip.length, oneShotObject));
   }

   IEnumerator deleteSourceAfterPlay(float seconds, GameObject sourceObject)
   {
      yield return new WaitForSeconds(seconds);
      Destroy(sourceObject);
   }
}
