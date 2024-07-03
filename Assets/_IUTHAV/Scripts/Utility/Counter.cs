using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _IUTHAV.Scripts.Utility {
    public class Counter : MonoBehaviour {

        [SerializeField] private int maxCount = 3;
        public UnityEvent onMaxCountReached;
        [SerializeField] private bool resetOnMaxReached;
        [SerializeField] private bool isDebug;

        private int _currentCount;

        private List<string> _keys;

        private void Awake() {
            _keys = new List<string>();
        }

        public void Increment(string key = "") {

            if (!key.Equals("")) {

                if (!_keys.Contains(key)) {
                    _keys.Add(key);
                }
                else {
                    Log("Didn't increment, already incremented with key: " + key);
                    return;
                }
            }
            
            _currentCount++;
            Log("Current count: " + _currentCount);

            if (_currentCount == maxCount) {
                
                onMaxCountReached.Invoke();
                _currentCount = resetOnMaxReached ? 0 : _currentCount;
                Log("MaxCount reached");

            }
        }
        
        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[Counter][" + gameObject.name + "]" + msg);
        }

    }
}
