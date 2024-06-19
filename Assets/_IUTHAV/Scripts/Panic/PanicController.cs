using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _IUTHAV.Scripts.Panic {
    public class PanicController : MonoBehaviour {

        [SerializeField] private GameObject[] panicables;

        private List<IPanicable> _panicables;
        private Queue<IEnumerator> _jobQueue = new Queue<IEnumerator>();
        private IEnumerator _currentJob;
        private float _globalPanicDelta = 3.0f;
        private bool _jobRunning;

        [SerializeField] private bool isDebug;


#region Helper Classes

        private class PanicJob {
            
            public float DeltaTime;
            public float TargetPanicValue;
            public PanicJob(float deltaTime, float targetPanicValue) {
                DeltaTime = deltaTime;
                TargetPanicValue = targetPanicValue;
            }
        }

#endregion

#region Unity Functions

        private void Awake() {

            Configure();
            StartCoroutine(CoroutineCoordinator());
        }

        private void OnDisable() {
            Dispose();
        }

#endregion

#region Public Functions

        /// <summary>
        /// Starts a coroutine, that tells all IPanicable objects to call their Lerp Functions during
        /// _globalPanicDelta seconds
        /// </summary>
        /// <param name="targetPanicValue">Value clamped between 0-1 with 1 representing maximum Panic</param>
        public void Panic(float targetPanicValue) {

            float v = Math.Clamp(targetPanicValue, 0.0f, 1.0f);
            AddJob(new PanicJob(_globalPanicDelta, v));
        }

        /// <summary>
        /// Upon changing this delta, all coroutines called afterwards will use newDelta
        /// making every coroutine take newDelta seconds to reach the _targetPanicValue
        /// </summary>
        /// <param name="newDelta">Float to override the currentDelta</param>
        public void ChangePanicDelta(float newDelta) {
            _globalPanicDelta = newDelta;
        }

#endregion

#region Private Functions

        /// <summary>
        /// Main Coroutine, which calls all Panicables Functions to lerp them
        /// </summary>
        /// <param name="job">Job to be run</param>
        /// <returns>True once the Coroutine is finished, otherwise null</returns>
        private IEnumerator RunPanicJob(PanicJob job) {
            
            Log(String.Format("Running job for {0} seconds to target {1}", job.DeltaTime, job.TargetPanicValue));
            
            float t = 0;
            
            //Set the starting and endpoint local variables for the lerp functions
            foreach (var obj in _panicables) {
                obj.SetBaseParameter();
                obj.SetTargetParameter(job.TargetPanicValue);
            }

            while (t < job.DeltaTime) {

                foreach (var obj in _panicables) {
                    //Lerp to set local variable _currentParameterValue 
                    obj.LerpByPanicValue(t / job.DeltaTime);
                    //Set the wanted Parameter to the previously Lerped value using _currentParameterValue
                    obj.SetDesiredParameter();
                }

                t += Time.deltaTime;
                yield return null;
            }
            Log("Finished job");
            yield return true;
        }

        /// <summary>
        /// Sub-Coroutine which makes sure, no 2 jobs are running at the same time
        /// </summary>
        /// <returns></returns>
        private IEnumerator CoroutineCoordinator() {

            while (_jobQueue.Count > 0) {
                var jobRunner = _jobQueue.Dequeue();
                _currentJob = jobRunner;
                yield return StartCoroutine(jobRunner);
            }

            _currentJob = null;
        }
        
        private void AddJob(PanicJob job) {

            _jobQueue.Enqueue(RunPanicJob(job));
            
            if (_currentJob == null) StartCoroutine(CoroutineCoordinator());
            Log("Queuing new job");
            
        }
        
        /// <summary>
        /// Populates _panicables List and discards GameObjects, which do not have any IPanicable Components
        /// </summary>
        private void Configure() {

            _panicables = new List<IPanicable>();
            foreach (var obj in panicables) {

                var panics = obj.GetComponents<IPanicable>();
                
                if (panics == null || panics.Length == 0) {
                    LogWarning("GameObject [" + obj.name + "] does not implement any IPanicables");
                }
                else {
                    foreach (var panic in panics) _panicables.Add(panic);
                }
            }
            
        }

        private void Dispose() {
            _jobQueue.Clear();

            if (_currentJob != null) {
                StopCoroutine(_currentJob);
            }
        }

        protected void Log(object msg) {
            if (!isDebug) return;
            Debug.Log("[PanicController]: " + msg.ToString());
        }
        protected void LogWarning(object msg) {
            if (!isDebug) return;
            Debug.LogWarning("[PanicController]: " + msg.ToString());
        }

#endregion
    }
}
