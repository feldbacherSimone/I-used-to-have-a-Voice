using _IUTHAV.Scripts.CustomUI;
using UnityEngine;
using UnityEngine.Playables;

namespace _IUTHAV.Scripts.Timeline {
    public class ScrollBackgroundReceiver : MonoBehaviour, INotificationReceiver {

        [SerializeField] private ScrollBackGround scrollBackground;

        private void Start() {
            if (scrollBackground == null) {
                scrollBackground = FindFirstObjectByType<ScrollBackGround>();
            }
        }
        
        public void OnNotify(Playable origin, INotification notification, object context) {
            
            if (scrollBackground == null) {
                Debug.LogError("Cannot receive Markersignal from ScrollBackground - check if a Scrollbackground is assigned in the receiver!");
                return;
            }
            
            ScrollBackgroundMarker marker = notification as ScrollBackgroundMarker;
            
            if (marker == null) {
                Debug.LogError("Wrong INotification received, aborting message");
                return;
            }

            switch (marker.CommandType) {
                
                case ScrollBackgroundCommandType.NextEndpoint:
                    scrollBackground.NextBookmark();
                    break;
                    
                case ScrollBackgroundCommandType.EnableUserScrolling:
                    scrollBackground.ToggleManualScroll(marker.EnableUserScrolling);
                    break;
                    
                case ScrollBackgroundCommandType.ForceScrollToEndpoint:
                    scrollBackground.ForceScrollToEndpoint(marker.LockOnScrollend, marker.ForceScrollSpeed);
                    break;
            
            }
            
        }
    }
}