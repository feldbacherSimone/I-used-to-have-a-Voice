using UnityEngine;
using UnityEngine.Playables;

namespace _IUTHAV.Scripts.Timeline.Lerpable {
    public class LerpableControlAsset  : PlayableAsset {

        public LerpableControlBehaviour template;
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<LerpableControlBehaviour>.Create(graph, template);
            return playable;
        }
    }
}