using System.Collections.Generic;
using UnityEngine;

namespace _IUTHAV.Scripts.Tilemap {
    public class Tile : MonoBehaviour {

        [SerializeField] private Transform endPoint;

        public Transform EndPoint => endPoint;

        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject[] spawnObjects;
        [SerializeField] private bool randomizeXRotation;
        [SerializeField] private bool randomizeYRotation;
        [SerializeField] private bool randomizeZRotation;
        
        private List<GameObject> _mSpawnedObjects;

        private void Awake() {

            if (spawnObjects == null || spawnPoints == null) return;

            _mSpawnedObjects = new List<GameObject>();

            foreach (var p in spawnPoints) {

                int random = Random.Range(0, spawnObjects.Length);

                var obj = Instantiate(spawnObjects[random], p, true);

                Vector3 rot = obj.transform.rotation.eulerAngles;

                if (randomizeXRotation) rot.x = Random.Range(0, 360);
                if (randomizeYRotation) rot.y = Random.Range(0, 360);
                if (randomizeZRotation) rot.z = Random.Range(0, 360);

                obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(rot));
                
                _mSpawnedObjects.Add(obj);

            }
            
        }

        public Vector3 GetLocalPosEndpoint() {

            return endPoint.localPosition;
        }

        public Quaternion GetLocalRotEndpoint() {
            
            return endPoint.localRotation;
        }

        public void DestroyTile() {

            foreach (var obj in _mSpawnedObjects) {
                
                Destroy(obj);
            }
            
        }

    }
}
