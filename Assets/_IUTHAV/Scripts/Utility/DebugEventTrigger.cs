using System;
using _IUTHAV.Scripts.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _IUTHAV.Scripts.Utility {
    public class DebugEventTrigger : MonoBehaviour {
        
        private void OnEnable() {
            InputController.OnDebug += ToggleDebugMenu;
        }
        
        private void ToggleDebugMenu(InputAction.CallbackContext context) {
            
            GameObject debugMenu = transform.GetChild(0)?.gameObject;
                
            debugMenu.SetActive(!debugMenu.activeSelf);
        }

        private void OnDisable() {
            InputController.OnDebug -= ToggleDebugMenu;
        }
    }
}