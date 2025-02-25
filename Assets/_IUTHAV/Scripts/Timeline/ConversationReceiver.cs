using UnityEngine;
using UnityEngine.Playables;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Timeline {
    public class ConversationReceiver : MonoBehaviour, INotificationReceiver {
        
        [SerializeField] private DialogueRunner dialogueRunner;
        
        private void Start() {
            if (dialogueRunner == null) {
                dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            }
        }
    
        public void OnNotify(Playable origin, INotification notification, object context) {
        
            if (notification is ConversationMarker conversationMarker && dialogueRunner != null) {
                dialogueRunner.StartDialogue(conversationMarker.ConversationName);
            }
            
        }
    }
}