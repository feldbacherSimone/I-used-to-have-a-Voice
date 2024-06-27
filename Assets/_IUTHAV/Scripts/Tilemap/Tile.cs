using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Tilemap {
    public class Tile : MonoBehaviour {

        [SerializeField] private Transform endPoint;
        [SerializeField] private GameObject tileObject;
        [SerializeField] private bool animateTile = true;

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
            
            if (animateTile) StartCoroutine(AnimateTileMesh(true));
            
        }

        public Transform GetEndpointTransform() {
            
            return endPoint.transform;
        }

        public void DestroyTile() {

            foreach (var obj in _mSpawnedObjects) {
                
                Destroy(obj);
            }
            
            StopAllCoroutines();
            
        }
        
        private IEnumerator AnimateTileMesh(bool enable) {

            float t = 0;
            
            Vector3 baseScale = enable ? Vector3.zero : tileObject.transform.localScale;
            Vector3 targetScale = !enable ? Vector3.zero : tileObject.transform.localScale;

            while (t < 4.0f) {

                tileObject.transform.localScale = Vector3.Lerp(baseScale, targetScale, t / 4.0f);

                t += Time.deltaTime;
                yield return null;
            }

        }

    }
}
