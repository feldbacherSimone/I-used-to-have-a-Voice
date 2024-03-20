using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _IUTHAV.Testing.DataPersistance {
    public class DataStartupManager : MonoBehaviour {

        [SerializeField] private DataTest sessionData;
        [SerializeField] private DataTest backupData;
        [SerializeField] private string startScene;
        private void Awake() {
            sessionData.highscore = backupData.highscore;
            SceneManager.LoadScene(startScene);
        }
    }
}
