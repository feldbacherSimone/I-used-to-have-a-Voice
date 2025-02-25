using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline {
    public class ConversationMarker : Marker, INotification, INotificationOptionProvider {
        
        [Tooltip("Insert the name of the required Dialogue here")]
        [SerializeField] private string conversationName;
        public string ConversationName => conversationName;
        
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
}