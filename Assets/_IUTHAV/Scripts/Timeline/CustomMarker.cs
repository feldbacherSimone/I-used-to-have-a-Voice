using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline {
    public abstract class CustomMarker : Marker, INotification, INotificationOptionProvider {

        [Header("Marker settings")]
        [Space(10)]
        [Tooltip("Use retroactive to emit the signal if playback starts after the SignalEmitter time.")]
        [SerializeField] private bool retroactive = true;
        [SerializeField] private bool emitOnce = false;
        
        public PropertyName id => new PropertyName();

        public NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (emitOnce ? NotificationFlags.TriggerOnce : default);
        
        [Header("Marker cosmetics")]
        public bool ShowMarkerOverlay = true;
        public bool ShowLineOverlay;
        public abstract Color GetColor();
    }
}