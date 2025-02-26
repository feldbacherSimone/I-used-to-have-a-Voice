using System;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Timeline.Lerpable {
    [Serializable]
    public class LerpableControlBehaviour : PlayableBehaviour {
       public float CurrentLerpvalue;
    }
}