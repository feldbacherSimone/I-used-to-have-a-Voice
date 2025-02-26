using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _IUTHAV.Scripts.Timeline {
    public class ConversationMarker : CustomMarker {
        
        [Tooltip("Insert the name of the required Dialogue here")]
        [SerializeField] private string conversationName;
        public string ConversationName => conversationName;


        public override Color GetColor() {
            return Color.green;
        }
    }
}