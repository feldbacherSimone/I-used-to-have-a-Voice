using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _IUTHAV.Scripts.Utility {
    public class Sequencer : MonoBehaviour {
        
#region Helper Classes

        [System.Serializable]
        public class Sequence {
            public UnityEvent onSequenceStart;
            public float delay;
        }
#endregion

        [SerializeField] private bool isDebug;
        [SerializeField] private bool isLoop;
        [SerializeField] private bool startOnEnable;
        [SerializeField] private Sequence[] sequences;


        private bool _isRunning;

#region Unity Functions

        private void OnEnable() {
            if (startOnEnable) {
                StartSequence();
            }
        }

        private void OnDisable() {
            StopSequence();
        }

#endregion

#region Public Functions

        public void StartSequence() {
            if (!_isRunning) StartCoroutine(RunSequence());
            _isRunning = true;
        }

        public void StopSequence() {
            StopAllCoroutines();
            _isRunning = false;
        }
        
#endregion

#region Private Functions

        private IEnumerator RunSequence() {
            Log("Starting Sequence");
            foreach (Sequence sequence in sequences) {
                sequence.onSequenceStart.Invoke();
                    yield return new WaitForSeconds(sequence.delay);
            }

            if (isLoop) {
                StartCoroutine(RunSequence());
            }
            else _isRunning = false;

        }
        
        private void Log(object msg) {
            if (!isDebug) return;
            Debug.Log("[Sequencer]: " + msg.ToString());
        }
        private void LogWarning(object msg) {
            if (!isDebug) return;
            Debug.LogWarning("[Sequencer]: " + msg.ToString());
        }
        
#endregion

    }
}
