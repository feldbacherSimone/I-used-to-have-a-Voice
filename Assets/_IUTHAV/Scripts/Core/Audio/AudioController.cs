using System;
using System.Collections;
using System.Collections.Generic;
using _IUTHAV.Scripts.Utility;
using UnityEngine;
using UnityEngine.Audio;

namespace _IUTHAV.Scripts.Core.Audio
{
    //This script is based on Simone Feldbacher´s implementation in "Somnia" (game available online)
    //This script is based on Renaissance Coders video "UNITY 2020 ADVANCED AUDIO MANAGER, Game Essentials (3/5)"
    public class AudioController : MonoBehaviour
    {
        [SerializeField] protected bool isDebug;
        [Header("Audio Propaerties")]
        [SerializeField] protected List<AudioTrack> tracks;

        protected Hashtable audioTable;
        private Hashtable _jobTable;
        private static AudioMixerGroup _masterGroup;
        private static AudioMixerGroup _musicGroup;
        private static AudioMixerGroup _dialogueGroup;
        private static AudioMixerGroup _sfxGroup;
        private static AudioMixerGroup _ambientGroup;
        
        [HideInInspector] public bool isReady;
        
        /// <summary>
        /// Helper class to bind Audiosources to multiple clips
        /// </summary>
        [System.Serializable]
        public class AudioTrack {
            public string name;
            public AudioSource source;
            [Range(0, 1)] public float volume = 1.0f;
            public AudioGroupType groupType;
            public List<AudioClip> audio;
            public bool autoSearch;
            public AutoClipFindSettings autoClipFindSettings;
        }

        [Serializable]
        public struct AutoClipFindSettings {
            public bool isSceneAudio;
            public string searchFilter;
        }

        /// <summary>
        /// Helper class to send dynamic objects through coroutines
        /// </summary>
        private class AudioJob {
            public AudioAction action;
            public string name;
            public bool fade;
            public float delay;
            public AudioJob(AudioAction _action, string _name, bool _fade, float _delay) {
                action = _action;
                name = _name;
                fade = _fade;
                delay = _delay;
            }
        }

        /// <summary>
        /// All possible Actions an audiotype can do
        /// </summary>
        private enum AudioAction {
            Start,
            Stop,
            Restart,
        }

#region Unity Functions
        private void Start() {

            Configure();
        }
        
        private void OnDisable() {
            Dispose();
        }

#endregion

#region public Functions

        public void PlayClip(string type, Transform spatialParent = null, bool fade = false, float delay = 0f) {
            
            if (spatialParent == null) {
                
                //track.source.spatialBlend = 0;
                PlayAudio(type, fade, delay);
                return;
            }
            AudioTrack track = GetTrackByClipName(type);
            GameObject obj = track.source.gameObject;
            obj.transform.parent = spatialParent;
            obj.transform.position = spatialParent.position;
            track.source.spatialBlend = 1;

            PlayAudio(type, fade, delay);
            
        }

        public virtual void PlayClip(AudioClip clip) {
            bool clipExists = false;
            foreach (AudioTrack track in tracks) {
                if (track.audio.Contains(clip)) {
                    clipExists = true;
                    break;
                }
            }

            if (!clipExists) {
                tracks[0].audio.Add(clip);
                GenerateAudioTable(tracks);
            }
            PlayClip(clip.name);
        }
        
        /// <summary>
        /// Plays an AudioType and returns it´s length in seconds
        /// If the track is already playing in the source, the newest track will be played
        /// </summary>
        /// <param name="type">Type of audio, should equal clip-name</param>
        /// <param name="spatialParent">Transform the soundsource should be parented to. Sets spatial to 1 if transform is given. Will stay as child, until another parent is given</param>
        /// <param name="fade">Optional parameter to decide if user wants fading</param>
        /// <param name="delay">Optional parameter to decide how long it should fade</param>
        /// <returns>Length of played clip in seconds</returns>
        public float PlayClipAndWait(string type, Transform spatialParent = null, bool fade = false,
            float delay = 0f) {
            AudioTrack track = (AudioTrack)audioTable[type];

            if (spatialParent == null) {
                track.source.spatialBlend = 0;
                PlayAudio(type, fade, delay);
                return GetAudioClipFromAudioTrack(type, track).length;
            }

            GameObject obj = track.source.gameObject;
            obj.transform.parent = spatialParent;
            obj.transform.position = spatialParent.position;
            track.source.spatialBlend = 1;

            PlayAudio(type, fade, delay);
            return GetAudioClipFromAudioTrack(type, track).length;
        }

        public static void UpdateMixerVolume() {
            _masterGroup.audioMixer.SetFloat(AudioGroupType.Master.ToString().ToLower() + "volume", Mathf.Log10(AudioOptionsController.MasterVolume) * 20);
            _musicGroup.audioMixer.SetFloat(AudioGroupType.Music.ToString().ToLower() + "volume", Mathf.Log10(AudioOptionsController.MusicVolume) * 20);
            _dialogueGroup.audioMixer.SetFloat(AudioGroupType.Dialogue.ToString().ToLower() + "volume", Mathf.Log10(AudioOptionsController.DialogueVolume) * 20);
            _sfxGroup.audioMixer.SetFloat(AudioGroupType.SFX.ToString().ToLower() + "volume", Mathf.Log10(AudioOptionsController.SFXVolume) * 20);
            _ambientGroup.audioMixer.SetFloat(AudioGroupType.Ambient.ToString().ToLower() + "volume", Mathf.Log10(AudioOptionsController.AmbientVolume) * 20);
        }

        public static void UpdateMixerFX() {
            //TODO: change to Log function, so there is a smoother transition
            _masterGroup.audioMixer.SetFloat(AudioGroupType.Master.ToString().ToLower() + "lowpasscuttofffreq",
                AudioFXController.Lowpasscutofffreq);
            _masterGroup.audioMixer.SetFloat(AudioGroupType.Master.ToString().ToLower() + "lowpassresonance",
                AudioFXController.Lowpassresonance);
            _masterGroup.audioMixer.SetFloat(AudioGroupType.Master.ToString().ToLower() + "echodelay",
                AudioFXController.Masterechodelay);
            _masterGroup.audioMixer.SetFloat(AudioGroupType.Master.ToString().ToLower() + "echodecay",
                AudioFXController.Masterechodecay);
        }
        
        
#endregion

#region private Functions

        protected void Configure() {

            audioTable = new Hashtable();
            _jobTable = new Hashtable();

            LoadMixerGroups();
            GenerateAudioTable(tracks);
            isReady = true;
        }
        
        protected void Dispose() {
            //Disable all coroutines
            
            foreach (DictionaryEntry entry in _jobTable) {
                IEnumerator job = (IEnumerator)entry.Value;
                StopCoroutine(job);
            }
        }

        protected void PlayAudio(string type, bool fade = false, float delay = 0.0f) {
            AddJob(new AudioJob(AudioAction.Start, type, fade, delay));
        }
        
        protected void Stop(string type, bool fade = false, float delay = 0.0f) {
            AddJob(new AudioJob(AudioAction.Stop, type, fade, delay));
        }
        
        protected void RestartAudio(string type, bool fade = false, float delay = 0.0f) {
            AddJob(new AudioJob(AudioAction.Restart, type, fade, delay));
        }

        /// <summary>
        /// Takes a type and track to give back the clip, that resides within the tracks Audiotype
        /// </summary>
        /// <param name="type">Type to search for</param>
        /// <param name="track">Track to search through</param>
        /// <returns></returns>
        protected AudioClip GetAudioClipFromAudioTrack(string type, AudioTrack track) {
            foreach (AudioClip obj in track.audio) {
                if (obj.name == type)
                {
                    return obj;
                }
            }
            return null;
        }
        
        protected void GenerateAudioTable(List<AudioTrack> _tracks) {
        
            foreach (AudioTrack track in _tracks) {
            
                CheckAutoLoad(track);
                
                AssignAudioGroup(track);
                foreach (AudioClip obj in track.audio) {
                    if (obj == null) continue;
                    if (audioTable.ContainsKey(obj.name)) {
                        LogWarning("Already registered audio ["+obj.name+"]");
                    }
                    else {
                        audioTable.Add(obj.name, track);
                        Log("Registered Audio ["+obj.name+"] in ["+ track.name + "]");
                    }
                }
            }
            
        }

        private void CheckAutoLoad(AudioTrack track) {

            if (track.autoSearch) {
                
                if (!ResourceSearch.IsReadyAudio) ResourceSearch.ConfigureAudio();
                List<AudioClip> clips = AutoClipFinder.GetAudioTracks(track.autoClipFindSettings);
                if (clips != null) track.audio = clips;
            }
        }

        protected void LoadMixerGroups() {
            AudioMixer mixer = (AudioMixer)Resources.Load("Mixer");
            
            if (_masterGroup == null) _masterGroup = mixer.FindMatchingGroups("Master")[0];
            if (_musicGroup == null) _musicGroup = mixer.FindMatchingGroups(AudioGroupType.Music.ToString())[0];
            if (_dialogueGroup == null) _dialogueGroup = mixer.FindMatchingGroups(AudioGroupType.Dialogue.ToString())[0];
            if (_sfxGroup == null) _sfxGroup = mixer.FindMatchingGroups(AudioGroupType.SFX.ToString())[0];
            if (_ambientGroup == null) _ambientGroup = mixer.FindMatchingGroups(AudioGroupType.Ambient.ToString())[0];
        }

        private void AssignAudioGroup(AudioTrack track) {
        
            switch (track.groupType) {
                case AudioGroupType.Music:
                    track.source.outputAudioMixerGroup = _musicGroup;
                    break;
                case AudioGroupType.Dialogue:
                    track.source.outputAudioMixerGroup = _dialogueGroup;
                    break;
                case AudioGroupType.SFX: 
                case AudioGroupType.UI: 
                    track.source.outputAudioMixerGroup = _sfxGroup;
                    break;
                case AudioGroupType.Ambient:
                    track.source.outputAudioMixerGroup = _ambientGroup;
                    break;
            }
            Log("Assigned " + track.name + " to mixer " + track.groupType);
            
        }

        private void AddJob(AudioJob job) {
            //removing conflicting jobs
            RemoveConflictingJobs(job.name);
            //start job
            IEnumerator jobRunner = RunAudioJob(job);
            _jobTable.Add(job.name, jobRunner);
            StartCoroutine(jobRunner);
            Log("Starting Job ["+job.name+"] with operation: ["+job.action+"]");
        }

        private IEnumerator RunAudioJob(AudioJob job) {
            yield return new WaitForSeconds(job.delay);
            
            AudioTrack track = (AudioTrack)audioTable[job.name];

            track.source.clip = GetAudioClipFromAudioTrack(job.name, track);
            
            switch (job.action) {
                case AudioAction.Start:
                    track.source.volume = track.volume;
                    track.source.Play();
                    break;
                
                case AudioAction.Stop:
                    if (!job.fade) {
                        track.source.Stop();
                    }
                    
                    break;
                
                case AudioAction.Restart:
                    track.source.Stop();
                    track.source.Play();
                    break;
            }

            if (job.fade) {
                float initialVolume = job.action == AudioAction.Start || job.action == AudioAction.Restart ? 0.0f : 1.0f;
                float targetVolume = initialVolume == 0 ? 1 : 0;
                float duration = 1.0f;
                float timer = 0.0f;
                
                while(timer <= duration) {
                    track.source.volume = Mathf.Lerp(initialVolume, targetVolume, timer / duration);
                    timer += Time.deltaTime;
                    yield return null;
                }

                if (job.action == AudioAction.Stop) {
                    track.source.Stop();
                }
            }
            _jobTable.Remove(job.name);
            
            yield return null;
        }
        private void RemoveConflictingJobs(string type) {
            if (_jobTable.ContainsKey(type)) {
                RemoveJob(type);
            }

            string conflictAudio = "";
            foreach (DictionaryEntry entry in _jobTable) {
                string key = (string)entry.Key;
                AudioTrack audioTrackInUse = (AudioTrack)audioTable[key];
                AudioTrack audioTrackNeeded = (AudioTrack)audioTable[type];
                if (audioTrackNeeded == audioTrackInUse)
                {
                    conflictAudio = key;
                }
            }
            if (conflictAudio != "") {
                RemoveJob(conflictAudio);
            }
        }

        private void RemoveJob(string type) {
            if (!_jobTable.ContainsKey(type)) {
                LogWarning("Cannot stop ["+type+"] because it´s not running");
                return;
            }

            IEnumerator runningJob = (IEnumerator)_jobTable[type];
            StopCoroutine(runningJob);
            _jobTable.Remove(type);
        }
        
        protected AudioTrack GetTrack(string name) {
            foreach (AudioTrack track in tracks) {
                if (track.name == name) {
                    return track;
                }
            }

            LogWarning("Chris didn´t find the shitty track [" + name + "] you were looking for");
            return null;
        }

        protected AudioTrack GetTrackByClipName(string clipName) {
            
            foreach (AudioTrack track in tracks) {

                foreach (var clip in track.audio) {
                    if (clip.name == clipName) {
                        return track;
                    }
                }
            }
            LogWarning("Chris didn´t find the shitty track [" + name + "] you were looking for");
            return null;
        }
        
        protected void Log(object msg) {
            if (!isDebug) return;
            Debug.Log("[Audio Controller]: " + msg.ToString());
        }
        protected void LogWarning(object msg) {
            if (!isDebug) return;
            Debug.LogWarning("[Audio Controller]: " + msg.ToString());
        }
        
#endregion
    }
}
