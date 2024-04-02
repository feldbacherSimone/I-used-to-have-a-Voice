using System.Collections;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Page {
    public class Page : MonoBehaviour {
        
        [SerializeField] private PageType pageType;
        public PageType PageType => pageType;

        [SerializeField] private bool isAnimated;
        public bool IsAnimated => isAnimated;

        [SerializeField] private string _targetState;
        public string TargetState => _targetState;
        
        [SerializeField] private bool isDebug;

        public static readonly string FLAG_ON = "On";
        public static readonly string FLAG_OFF = "Off";
        public static readonly string FLAG_NONE = "None";

        private Animator _mAnimator;
        private bool _mIsOn;
        public bool IsOn => _mIsOn;
        
#region Unity Functions

        private void OnEnable() {
            Configure();
        }

#endregion

#region Public Functions

        public void Animate(bool on) {
            if (isAnimated) {
                _mAnimator.SetBool("on", on);
                
                StopCoroutine("AwaitAnimation");
                StartCoroutine("AwaitAnimation", on);
            }
            else {
                if (!on) {
                    gameObject.SetActive(false);
                    _mIsOn = false;
                }
                else {
                    _mIsOn = true;
                }
            }
        }

#endregion

#region Private Functions

        private void Configure() {
            if (isAnimated) {
                _mAnimator = GetComponent<Animator>();
                if (_mAnimator == null) {
                    LogWarning("Animator Component not found in this GameObject!");
                }
            }
        }

        private IEnumerator AwaitAnimation(bool on) {
            _targetState = on ? FLAG_ON : FLAG_OFF;
            while (!_mAnimator.GetCurrentAnimatorStateInfo(0).IsName(_targetState)) {
                yield return null;
            }
            
            while (_mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {
                yield return null;
            }

            _targetState = FLAG_NONE;

            if (!on) {
                _mIsOn = false;
            }

            yield return null;
        }

        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[Page" + gameObject.name + "] " + msg);
        }

#endregion        
        
        
    }
}
