using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline {
    public class ScrollBackgroundMarker : Marker, INotification, INotificationOptionProvider {
        
        [Tooltip("Insert the name of the required Dialogue here")]
        [SerializeField] private ScrollBackgroundCommandType commandType;
        public ScrollBackgroundCommandType CommandType => commandType;

        [ShowIf("commandType", ScrollBackgroundCommandType.ForceScrollToEndpoint)]
        [SerializeField]
        [Range(1, 16)]
        private float forceScrollSpeed = 4.0f;

        public float ForceScrollSpeed => forceScrollSpeed;

        [ShowIf("commandType", ScrollBackgroundCommandType.ForceScrollToEndpoint)] [SerializeField]
        [Tooltip("Enable, if you want to automatically disable scrolling after the script scrolled to the next endpoint")]
        private bool lockOnScrollEnd;

        public bool LockOnScrollend => lockOnScrollEnd;

        [ShowIf("commandType", ScrollBackgroundCommandType.EnableUserScrolling)]
        [Tooltip("Mark as true, if users should be able to scroll again")]
        [SerializeField] private bool enableUserScrolling;

        public bool EnableUserScrolling => enableUserScrolling;
        
        [Header("Marker settings")]
        [Space(10)]
        [Tooltip("Use retroactive to emit the signal if playback starts after the SignalEmitter time.")]
        [SerializeField] private bool retroactive = true;
        [SerializeField] private bool emitOnce = false;
        
        public PropertyName id => new PropertyName();

        public NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (emitOnce ? NotificationFlags.TriggerOnce : default);
    }
    
    public enum ScrollBackgroundCommandType {
            NextEndpoint,
            ForceScrollToEndpoint,
            EnableUserScrolling,
        }

    
}