using System;
using UnityEngine;

namespace _IUTHAV.Scripts.Tilemap {
    public class WireController : MonoBehaviour {

        [SerializeField] private GameObject[] startPoints;
        [SerializeField] private GameObject[] endPoints;
        [SerializeField] private Wire[] wires;
        public string key;

        [HideInInspector] public WireController connection;

        private WireController _mForwardctrl;

        private void Start() {

            if (!IsValid()) return;

            for (int i = 0; i < wires.Length; i++) {

                wires[i].start = startPoints[i];
                
            }

            WireController[] wireControllers = FindObjectsByType<WireController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            if (wireControllers == null || wireControllers.Length == 0) return;

            foreach (var ctrl in wireControllers) {

                if (ctrl != this && ctrl.connection == null && key.Equals(ctrl.key)) {
                    
                    _mForwardctrl = ctrl;
                    GenerateWires(_mForwardctrl.endPoints);
                    _mForwardctrl.connection = this;
                    break;
                }
            }
        }

        private void OnDestroy() {
            if (connection != null) {
                connection.RemoveWires();
            }
            
        }

        public void GenerateWires(GameObject[] _endPoints) {

            for (int i = 0; i < wires.Length; i++) {

                wires[i].GenerateWire(_endPoints[i]);

            }
        }

        public void RemoveWires() {

            foreach (var wire in wires) {
                wire.Disable();
            }
            
        }

        private bool IsValid() {

            bool valid = true;
            
            if (startPoints.Length != endPoints.Length) {
                Debug.LogError("Must have same number of Endpoints as Startpoints!");
                valid = false;
            }

            if (startPoints.Length != wires.Length) {
                Debug.LogError("Must have same number of wires as Startpoints!");
                valid = false;
            }
            
            return valid;
        }

    }
}
