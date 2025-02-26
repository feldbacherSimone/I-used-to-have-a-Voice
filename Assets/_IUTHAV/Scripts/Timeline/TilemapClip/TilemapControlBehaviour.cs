using System;
using UnityEngine.Playables;

namespace _IUTHAV.Scripts.Timeline.TilemapClip {
    [Serializable]
    public class TilemapControlBehaviour : PlayableBehaviour {
       public float CurrentPosition;
       public int CurrentTileIndex;
    }
}