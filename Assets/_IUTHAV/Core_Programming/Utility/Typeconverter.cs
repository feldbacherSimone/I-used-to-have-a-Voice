using System;
using _IUTHAV.Core_Programming.Gamemode;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Utility {
    
    /// <summary>
    /// Utility class to parse strings into Enums
    /// Additionally, this class provides context-appropriate states
    /// by changing a states Pre/Suffix to match it's scene and use-case
    /// </summary>

    public static class Typeconverter {
        
        public static StateType ChangePreAndSuffix(StatePrefix statePrefix, StateType exampleState, int suffix = -1) {
            
            string stateId = exampleState.ToString().Split("_")[1];
            string stateString = statePrefix + "_" + stateId;

            if (suffix >= 0) {
                stateString += ("_" + suffix);
            }

            if (Enum.TryParse(stateString, out StateType type)) {
                return type;
            }
            
            LogWarning("Couldn't parse StateType with identification [" + stateId + "] !");
            return StateType.None;
        }
        private static void LogWarning(string msg) {
            Debug.LogWarning("[Typeconverter] " + msg);
        }
        
    }
}
