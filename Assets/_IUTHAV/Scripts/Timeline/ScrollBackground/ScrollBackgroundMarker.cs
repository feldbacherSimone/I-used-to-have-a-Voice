using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline.ScrollBackground {

    public class ScrollBackgroundMarker : CustomMarker {
        
        [Space(20)]
        [Header("Scrollbackground Settings")]
        [Tooltip("Select which type of command this marker emmits")] 
        [SerializeField]
        private ScrollBackgroundCommandType commandType;
        
        public ScrollBackgroundCommandType CommandType => commandType;
        
        //TODO: Find out why NaughtyAttributes doesnt work in this case
        [Range(1, 16)]
        [ShowIf("commandType", ScrollBackgroundCommandType.ForceScrollToEndpoint)]
        [SerializeField]
        //[ShowIf("commandType", ScrollBackgroundCommandType.ForceScrollToEndpoint)] 
        private float forceScrollSpeed = 4.0f;

        public float ForceScrollSpeed => forceScrollSpeed;
        
        [SerializeField]
        [Tooltip(
            "Enable, if you want to automatically disable scrolling after the script scrolled to the next endpoint")]
        [ShowIf("commandType", ScrollBackgroundCommandType.ForceScrollToEndpoint)]
        private bool lockOnScrollEnd;

        public bool LockOnScrollend => lockOnScrollEnd;
        
        [Tooltip("Mark as true, if users should be able to scroll again")]
        [SerializeField]
        [ShowIf("commandType", ScrollBackgroundCommandType.EnableUserScrolling)]
        private bool enableUserScrolling;

        public bool EnableUserScrolling => enableUserScrolling;

        public enum ScrollBackgroundCommandType {
            NextEndpoint,
            ForceScrollToEndpoint,
            EnableUserScrolling,
        }


        public override Color GetColor() {
            return Color.magenta;
        }
    }
}