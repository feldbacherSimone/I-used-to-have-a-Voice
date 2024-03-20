using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Testing.DataPersistance {
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataTest", order = 1)]
    public class DataTest : ScriptableObject {

        public int highscore;

    }
}
