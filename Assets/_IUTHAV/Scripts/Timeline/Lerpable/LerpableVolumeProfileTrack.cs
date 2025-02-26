using _IUTHAV.Scripts.Lerpables;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline.Lerpable {
    
    [TrackColor(0.1f, 0.6f, 0.8f)]
    [TrackClipType(typeof(LerpableControlAsset))]
    [TrackBindingType(typeof(LerpToonShaderParameter))]
    public class LerpableToonShaderTrack : TrackAsset {
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<LerpableControlMixerBehaviour>.Create(graph, inputCount);
        }
        
    }
    
    [TrackColor(0.1f, 0.7f, 0.8f)]
    [TrackClipType(typeof(LerpableControlAsset))]
    [TrackBindingType(typeof(LerpColourParameter))]
    public class LerpableColourTrack : TrackAsset {
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<LerpableControlMixerBehaviour>.Create(graph, inputCount);
        }
        
    }
    
    [TrackColor(0.1f, 0.7f, 0.7f)]
    [TrackClipType(typeof(LerpableControlAsset))]
    [TrackBindingType(typeof(LerpTransformParameter))]
    public class LerpableTransformTrack : TrackAsset {
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<LerpableControlMixerBehaviour>.Create(graph, inputCount);
        }
        
    }
    
    [TrackColor(0.1f, 0.6f, 0.7f)]
    [TrackClipType(typeof(LerpableControlAsset))]
    [TrackBindingType(typeof(LerpVolumeProfileParameter))]
    public class LerpableVolumeProfileTrack : TrackAsset {
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<LerpableControlMixerBehaviour>.Create(graph, inputCount);
        }
        
    }
}