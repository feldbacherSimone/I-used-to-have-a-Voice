using UnityEngine;
using Random = System.Random;

namespace _IUTHAV.Scripts.Core.Audio {
    public class AudioClipRandomizer : AudioController {
    
        private int _bufferIndex;
        
        private void Awake() {
            //TODO: Read in Audio table from AudioClip list
            base.Configure();
        }

        private void OnDisable() {
            base.Dispose();
        }
        
        /// <summary>
        /// Plays an Audiotype and offers additional parameters for effects
        /// If the track is already playing in the source, the newest track will be played
        /// </summary>
        /// <param name="type">Type of audio, should equal clip-name</param>
        /// <param name="spatialParent">Transform the soundsource should be parented to. Sets spatial to 1 if transform is given. Will stay as child, until another parent is given</param>
        /// <param name="fade">Optional parameter to decide if user wants fading</param>
        /// <param name="delay">Optional parameter to decide how long it should fade</param>
        public void PlayRandomClip(string trackName, Transform spatialParent = null, bool fade = false,
            float delay = 0f) {

            AudioTrack track = GetTrack(trackName);
            if (track == null) return;
            
            //ensure no sound is played twice
            Random random = new Random();
            int index;
            do {
                index = random.Next(0, track.audio.Count);
            } while (index == _bufferIndex);

            _bufferIndex = index;
            string type = track.audio[index].name;
            Log("Playing Clip Nr." + index);
            if (spatialParent == null) {
                track.source.spatialBlend = 0;

                PlayAudio(type, fade, delay);
                return;
            }

            GameObject obj = track.source.gameObject;
            obj.transform.parent = spatialParent;
            obj.transform.position = spatialParent.position;
            track.source.spatialBlend = 1;

            PlayAudio(type, fade, delay);
        }

        public void PlayRandomClip(string name) {
            
            PlayRandomClip(name, null);
            
        }

        /// <summary>
        /// Plays an Audiotype and offers additional parameters for effects
        /// If the track is already playing in the source, the newest track will be played
        /// </summary>
        /// <param name="type">Type of audio, should equal clip-name</param>
        /// <param name="spatialParent">Transform the soundsource should be parented to. Sets spatial to 1 if transform is given. Will stay as child, until another parent is given</param>
        /// <param name="fade">Optional parameter to decide if user wants fading</param>
        /// <param name="delay">Optional parameter to decide how long it should fade</param>
        /// <returns>Length of played clip in seconds</returns>
        public float PlayRandomClipAndWait(string trackName, Transform spatialParent = null, bool fade = false,
            float delay = 0f) {
            
            AudioTrack track = GetTrack(trackName);
            if (track == null) return 0f;
            //ensure no sound is played twice in a row
            Random random = new Random();
            int index;
            do {
                index = random.Next(0, track.audio.Count);
            } while (index == _bufferIndex);

            _bufferIndex = index;
            string type = track.audio[index].name;
            Log("Playing Clip Nr." + index);
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
        
    }
}