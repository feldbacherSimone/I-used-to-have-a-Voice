using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline.ScrollTerrain {

    [TrackColor(0.4f, 0.1f, 0.1f)]
    [TrackBindingType(typeof(Material))]
    [TrackClipType(typeof(ScrollTerrainClip))]
    public class ScrollTerrainControlTrack : TrackAsset {

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject gameObject, int inputCount) {
            return ScriptPlayable<ScrollTerrainControlMixerBehaviour>.Create(graph, inputCount);
        }
        
    }
}