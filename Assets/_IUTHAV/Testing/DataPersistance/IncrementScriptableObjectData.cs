using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Testing.DataPersistance {
    public class IncrementScriptableObjectData : MonoBehaviour {

        [SerializeField] private DataTest dataTest;
        [SerializeField] private TMP_Text txt;

        private void Update() {
            txt.text = dataTest.highscore.ToString();
        }

        public void IncrementHighScore() {
            dataTest.highscore++;
        }
        
    }
}
