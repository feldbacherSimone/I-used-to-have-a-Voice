using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace _IUTHAV.Scripts.Core.Audio {
    public class AudioTrackLooper : AudioController {
        
        protected Hashtable _currentlyLooping;

        public enum LoopingMode {
            None,
            Single,
            Queued,
            Mixed,
        }

        private class LoopingObject {
            public int index;
            public LoopingMode loopingMode;

            public LoopingObject(int _index, LoopingMode _loopingMode) {
                index = _index;
                loopingMode = _loopingMode;
            }
        }
        
#region Unity Functions

        private void Awake() {

            Configure();
        }

        private void OnDisable() {
            base.Dispose();
        }

#endregion

#region Public Functions

        /// <summary>
        /// Plays an AudioType and repeats it until it is stopped with StopTrack or StopClip
        /// </summary>
        /// <param name="type">Type of audio, should equal clip-name</param>
        /// <param name="spatialParent">Transform the soundsource should be parented to. Sets spatial to 1 if transform is given. Will stay as child, until another parent is given</param>
        /// <param name="fade">Optional parameter to decide if user wants fading</param>
        /// <param name="delay">Optional parameter to decide how long it should fade</param>
        public void PlayClipOnLoop(string type, Transform spatialParent = null, bool fade = false,
            float delay = 0f, float initialDelay = 0f, bool initialFade = false) {
            AudioTrack track = (AudioTrack)audioTable[type];
            if (_currentlyLooping.ContainsKey(track)) {
                LogWarning("Chris already put on this sick beat: " + track);
                return;
            }

            if (spatialParent == null) {
                track.source.spatialBlend = 0;
            }
            else {
                GameObject obj = track.source.gameObject;
                obj.transform.parent = spatialParent;
                obj.transform.position = spatialParent.position;
                track.source.spatialBlend = 1;
            }

            LoopingObject loopingObject = new LoopingObject(0, LoopingMode.Single);
            IEnumerator loopJob = PlayClipCoroutine(type, track, fade, delay, loopingObject, initialDelay, initialFade);
            _currentlyLooping.Add(track, loopJob);
            StartCoroutine(loopJob);
        }

        /// <summary>
        /// Plays random clips within a track until it is stopped with StopTrack or StopClip
        /// </summary>
        /// <param name="track">Type of audio, should equal clip-name</param>
        /// <param name="spatialParent">Transform the soundsource should be parented to. Sets spatial to 1 if transform is given. Will stay as child, until another parent is given</param>
        /// <param name="fade">Optional parameter to decide if user wants fading</param>
        /// <param name="delay">Optional parameter to decide how long it should fade</param>
        /// <param name="initialDelay"></param>
        public void PlayTrackLoopRandom(string trackName, Transform spatialParent = null, bool fade = false,
            float delay = 0f, float initialDelay = 0f, bool initialFade = false) {
            
            AudioTrack track = GetTrack(trackName);
            if (track == null) {
                LogWarning("No Track found with name " + trackName);
                return;
            }
            
            if (_currentlyLooping.ContainsKey(track)) {
                LogWarning("Chris already put on this sick random beat: " + track);
                return;
            }

            if (spatialParent == null) {
                track.source.spatialBlend = 0;
            }
            else {
                GameObject obj = track.source.gameObject;
                obj.transform.parent = spatialParent;
                obj.transform.position = spatialParent.position;
                track.source.spatialBlend = 1;
            }
            if (track.audio.Count == 0) return;
            LoopingObject loopingObject = new LoopingObject(0, LoopingMode.Mixed);
            IEnumerator loopJob = PlayClipCoroutine(track.audio[0].name, track, fade, delay, loopingObject, initialDelay, initialFade);
            _currentlyLooping.Add(track, loopJob);
            StartCoroutine(loopJob);
        }

        public virtual void PlayTrackLoopRandom(string trackName) {
            PlayTrackLoopRandom(trackName, null);
        }

        /// <summary>
        /// Plays clips within a track by their order until it is stopped with StopTrack or StopClip
        /// </summary>
        /// <param name="track">Type of audio, should equal clip-name</param>
        /// <param name="spatialParent">Transform the soundsource should be parented to. Sets spatial to 1 if transform is given</param>
        /// <param name="fade">Optional parameter to decide if user wants fading</param>
        /// <param name="delay">Optional parameter to decide how long it should fade</param>
        public void PlayTrackLoopQueue(string trackName, Transform spatialParent = null, bool fade = false,
            float delay = 0f,float initialDelay = 0f, bool initialFade = false) {

            AudioTrack track = GetTrack(trackName);
            if (track == null) {
                LogWarning("No Track found with name " + trackName);
                return;
            }
            
            if (_currentlyLooping.ContainsKey(track)) {
                LogWarning("Chris already put on this sick beat: " + track.name);
                return;
            }

            if (spatialParent == null) {
                track.source.spatialBlend = 0;
            }
            else {
                GameObject obj = track.source.gameObject;
                obj.transform.parent = spatialParent;
                obj.transform.position = spatialParent.position;
                track.source.spatialBlend = 1;
            }
            if (track.audio.Count == 0) return;
            Log("Chris is putting on a queued track");
            LoopingObject loopingObject = new LoopingObject(0, LoopingMode.Queued);
            IEnumerator loopJob = PlayClipCoroutine(track.audio[0].name, track, fade, delay, loopingObject, initialDelay, initialFade);
            _currentlyLooping.Add(track, loopJob);
            StartCoroutine(loopJob);
        }

        public virtual void PlayTrackLoopQueue(string trackName) {
            PlayTrackLoopQueue(trackName, null, false, 0f, 0f);
        }

        /// <summary>
        /// Stops all clips within a track, including looping tracks. Has multiple Overloads
        /// </summary>
        /// <param name="spatialParent">Transform the soundsource should be parented to. Sets spatial to 1 if transform is given</param>
        /// <param name="fade">Optional parameter to decide if user wants fading</param>
        /// <param name="delay">Optional parameter to decide how long it should fade</param>
        public void StopAudioTrack(string trackName, bool fade = false, float delay = 0f,float initialDelay = 0f) {
            
            AudioTrack track = GetTrack(trackName);
            if (track == null) {
                LogWarning("No Track found with name " + trackName);
                return;
            }
            
            RemoveLoop(track);
            foreach (AudioClip obj in track.audio) {
                Stop(obj.name, fade, delay);
                Log("Stopping Audio " + obj.name);
            }
        }

        public virtual void StopAudioTrack(string trackName) {
            StopAudioTrack(trackName, false);
        }

        public void StopAudio(string type, bool fade = false, float delay = 0f,float initialDelay = 0f) {
            AudioTrack track = (AudioTrack)audioTable[type];
            RemoveLoop(track);
            Stop(type, fade, delay);
        }

        /// <summary>
        /// Plays a desired clip from the beginning
        /// </summary>
        /// <param name="spatialParent">Transform the soundsource should be parented to. Sets spatial to 1 if transform is given</param>
        /// <param name="fade">Optional parameter to decide if user wants fading</param>
        /// <param name="delay">Optional parameter to decide how long it should fade</param>
        public void RestartClip(string type, bool fade = false, float delay = 0f,float initialDelay = 0f) {
            AudioTrack track = (AudioTrack)audioTable[type];
            RemoveLoop(track);
            Stop(type, fade, delay);
            PlayAudio(type, fade, delay);
        }

        

#endregion

#region Private Functions

        private IEnumerator PlayClipCoroutine(string type, AudioTrack track, bool fade, float delay,
            LoopingObject mode, float initialDelay, bool initialFade) {
            
            float cliplength;

            bool exit = false;
            
            while (!exit) {

                switch (mode.loopingMode) {
                    case LoopingMode.Single:
                        track.source.loop = true;
                        PlayAudio(type, fade || initialFade, initialDelay);
                        exit = true;
                        break;

                    case LoopingMode.Queued:

                        PlayAudio(track.audio[mode.index].name, fade || initialFade, initialDelay);
                        cliplength = GetAudioClipFromAudioTrack(track.audio[mode.index].name, track).length;
                        if (mode.index < track.audio.Count - 1) {
                            mode.index++;
                        }
                        else {
                            mode.index = 0;
                        }

                        yield return new WaitForSeconds(cliplength+ initialDelay);
                        break;

                    case LoopingMode.Mixed:

                        Random random = new Random();
                        int index = 0;
                        if (track.audio.Count > 1) {
                            do {
                                index = random.Next(0, track.audio.Count);
                            } while (mode.index == index);
                        }

                        mode.index = index;
                        cliplength = GetAudioClipFromAudioTrack(track.audio[mode.index].name, track).length;
                        PlayAudio(track.audio[mode.index].name, fade || initialFade, initialDelay);
                        yield return new WaitForSeconds(cliplength+ initialDelay);
                        break;

                    default:
                        LogWarning("Chris doesn´t know what the fuck [" + mode.loopingMode + "] is");
                        break;
                }

                initialDelay = delay;
                initialFade = false;
            }
        }

        private void RemoveLoop(AudioTrack track) {
            if (track != null && !_currentlyLooping.ContainsKey(track)) {
                LogWarning("Cannot remove the Chris: [" + track.name + "] because it´s not running");
                return;
            }

            if (track != null) {
                IEnumerator runningJob = (IEnumerator)_currentlyLooping[track];
                StopCoroutine(runningJob);
                _currentlyLooping.Remove(track);
            }
            
        }

        protected new void Configure() {
            _currentlyLooping = new Hashtable();
            base.Configure();
        }

#endregion
    }
}