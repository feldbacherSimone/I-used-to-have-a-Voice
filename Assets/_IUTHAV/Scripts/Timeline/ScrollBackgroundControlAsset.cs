using UnityEngine;
using UnityEngine.Playables;

namespace _IUTHAV.Scripts.Timeline {
    public class ScrollBackgroundControlAsset  : PlayableAsset {

        public ScrollBackgroundControlBehaviour template;
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<ScrollBackgroundControlBehaviour>.Create(graph, template);
            return playable;
        }
    }
}