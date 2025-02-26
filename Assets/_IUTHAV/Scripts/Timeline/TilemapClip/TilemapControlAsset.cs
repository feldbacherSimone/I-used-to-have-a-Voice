using UnityEngine;
using UnityEngine.Playables;

namespace _IUTHAV.Scripts.Timeline.TilemapClip {
    public class TilemapControlAsset : PlayableAsset {

        public TilemapControlBehaviour template;
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<TilemapControlBehaviour>.Create(graph, template);
            return playable;
        }
    }
}