using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using GameObject = UnityEngine.GameObject;

namespace _IUTHAV.Scripts.Core.Audio
{
    
    public static class SoundManager 
    {
        public enum SoundType
        {
            UIHover, 
            UIClick,
       
            AnswerPickUp,
            AnswerDrop,
            AnswerBlocked,
       
            PhoneRing,
            TrainArrive,
            PhoneSend,
            TrainDoorOpen,
            TrainDoorClosed,
            TrainDoorButton,
       
        }
        public enum Mixer
        {
            SFX,
            Music,
        }

        const int defaultOneShotObjectCount = 10;
        private static List<OneShotHandler> ObjectPool = new List<OneShotHandler>();

        // Todo: move this
        private static GameObject oneShotGameObject;
        private static AudioSource oneShotAudioSource;

        private static Dictionary<SoundType, int> lastPlayedClipIndex = new();
        
        private static AudioMixer audioMixer;


        private static GameObject oneShotSourceParent;
        
        // API Methods
        public static void SetupAudioManager(int? oneShotObjectCount) 
        { 
            audioMixer = Resources.Load<AudioMixer>("Mixer"); 
            Debug.Log($"Setup: loadedMixer, {audioMixer}");

           CreateObjectPool(oneShotObjectCount);
           Debug.Log($"Setup: Setup finished");
        }

        public static void PlaySound(SoundType soundType, Mixer mixer)
        {
            OneShotHandler currentOneShotHandler = GetOneShotHandler();
            AssignMixer(mixer, currentOneShotHandler.getAudioSource());
            currentOneShotHandler.getAudioSource().volume = GetVolume(soundType);
            currentOneShotHandler.PlayOneShotClip(GetAudioClip(soundType));
        }
        
        
        // Internal Methods


        private static OneShotHandler GetOneShotHandler()
        {
            foreach (OneShotHandler oneShotHandler in ObjectPool)
            {
                if (oneShotHandler.IsAvailable()) return oneShotHandler; 
            }

            return CreateNewObjectInPool(); 
        }

        private static OneShotHandler CreateNewObjectInPool()
        {
            GameObject sourceObject = new GameObject($"OneShotSource_{ObjectPool.Count}");
            sourceObject.transform.parent = oneShotSourceParent.transform;
            sourceObject.AddComponent(typeof(OneShotHandler));
            sourceObject.AddComponent(typeof(AudioSource));
            sourceObject.GetComponent<OneShotHandler>().setAudioSource(sourceObject.GetComponent<AudioSource>());
            ObjectPool.Add(sourceObject.GetComponent<OneShotHandler>());

            return sourceObject.GetComponent<OneShotHandler>();
        }
        private static void CreateObjectPool(int? oneShotObjectCount)
        {
            oneShotSourceParent = new GameObject("OneShotParent");

            int objectCount = oneShotObjectCount ?? defaultOneShotObjectCount;
            for (int i = 0; i < objectCount; i++)
            {
               CreateNewObjectInPool();
            }
        }
        
        
        private static AudioClip GetAudioClip(SoundType soundType)
        {
            foreach (SoundAssets.SoundAudioClip soundAudioClips in SoundAssets.instance.soundAudioClips)
            {
                if (soundAudioClips.soundType == soundType)
                {
                    int clipIndex = 0;

                    if (soundAudioClips.audioClips.Length > 1) {
                        do
                        {
                            clipIndex = Random.Range(0, soundAudioClips.audioClips.Length);
                        } while (!isUniqueSoundIndex(clipIndex, soundType));
                    }
                    lastPlayedClipIndex[soundType] = clipIndex;

                    return soundAudioClips.audioClips[clipIndex];
                }
            }

            Debug.LogError("You messed up, there's no " + soundType + " audioClips");
            return null;
        }

        private static bool isUniqueSoundIndex(int clipIndex, SoundType soundType)
        {
            return !lastPlayedClipIndex.ContainsKey(soundType) || clipIndex != lastPlayedClipIndex[soundType];
        }
        private static float GetVolume(SoundType soundType)
        {
            foreach (SoundAssets.SoundAudioClip soundAudioClip in SoundAssets.instance.soundAudioClips)
            {
                if (soundAudioClip.soundType == soundType)
                {
                    return soundAudioClip.volume;
                }
            }
            Debug.LogError("you fucked up bro, there's no" + soundType + "Volume");
            return -1;
        }

        private static void AssignMixer(Mixer mixer, AudioSource source)
        {
            if (audioMixer != null)
                source.outputAudioMixerGroup = audioMixer.FindMatchingGroups(mixer.ToString())[0]; 
            else
                Debug.LogError("You Dumb Fuck, there is no Mixer to Accesess");
        }
    }
}