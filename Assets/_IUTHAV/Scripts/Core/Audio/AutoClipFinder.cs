using System.Collections.Generic;
using _IUTHAV.Scripts.Utility;
using UnityEngine;

namespace _IUTHAV.Scripts.Core.Audio {

    public static class AutoClipFinder {
        
        private static List<AudioClip> _audioTracks;
        
        public static List<AudioClip> GetAudioTracks(AudioController.AutoClipFindSettings settings) {

            _audioTracks = ResourceSearch.GetAllClips(settings.isSceneAudio, settings.searchFilter);

            return _audioTracks;
        }
    }
}