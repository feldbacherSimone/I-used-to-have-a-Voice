using UnityEngine;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Utility {
    public class YarnAnimationTrigger : MonoBehaviour {

        [SerializeField] private Animator animator;

        [YarnCommand("triggerAnimation")]
        public void TriggerAnimation(string trigger) {
        
            animator.SetTrigger(trigger);
        
        }

    }
}
