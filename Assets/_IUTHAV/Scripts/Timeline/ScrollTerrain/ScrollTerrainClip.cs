using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline.ScrollTerrain {

    [Serializable]
    public class ScrollTerrainClip : PlayableAsset, ITimelineClipAsset {

        [SerializeField] private ScrollTerrainBehaviour template = new ScrollTerrainBehaviour();
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            return ScriptPlayable<ScrollTerrainBehaviour>.Create(graph, template);
        }

        public ClipCaps clipCaps {
            get {
                return ClipCaps.Blending;
            }
        }
    }
}