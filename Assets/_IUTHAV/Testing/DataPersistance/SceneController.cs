using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _IUTHAV.Testing.DataPersistance {
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SceneController", order = 2)]
    public class SceneController : ScriptableObject {

        public List<Scene> scenes;
        
        public static void OpenScene(string scene) {
            SceneManager.LoadScene(scene);
        }
        
    }
    
}
