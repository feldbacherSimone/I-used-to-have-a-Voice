using _IUTHAV.Scripts.CustomUI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline {
    
    [TrackClipType(typeof(ScrollBackgroundControlAsset))]
    [TrackBindingType(typeof(ScrollBackGround))]
    public class ScrollBackgroundTrack : TrackAsset {
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<ScrollBackgroundControlMixerBehaviour>.Create(graph, inputCount);
        }
        
    }
}