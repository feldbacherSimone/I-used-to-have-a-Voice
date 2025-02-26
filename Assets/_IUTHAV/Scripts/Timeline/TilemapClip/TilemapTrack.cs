using _IUTHAV.Scripts.Tilemap;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline.TilemapClip {
    
    [TrackColor(0.3f, 0.3f, 1f)]
    [TrackClipType(typeof(TilemapControlAsset))]
    [TrackBindingType(typeof(TileController))]
    public class TilemapTrack : TrackAsset {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<TilemapMixerBehaviour>.Create(graph, inputCount);
        }
    }
}