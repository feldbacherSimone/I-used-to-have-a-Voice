using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _IUTHAV.Scripts.Tilemap {
    public class Tile : MonoBehaviour {

        [SerializeField] private Transform endPoint;
        [SerializeField] private GameObject tileObject;
        [SerializeField] private bool animateTile = true;

        public Transform EndPoint => endPoint;

        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject[] spawnObjects;
        public GameObject[] SpawnObjects => spawnObjects;
        [SerializeField] private bool randomizeXRotation;
        [SerializeField] private bool randomizeYRotation;
        [SerializeField] private bool randomizeZRotation;
        
        public Action<Transform, GameObject> OnRequestSpawnObjectChild;

        private void OnEnable() {

            if (spawnObjects == null || spawnPoints == null) return;
            
            foreach (var p in spawnPoints) {
                int random = Random.Range(0, spawnObjects.Length);
                var obj = spawnObjects[random];
                
                OnRequestSpawnObjectChild.Invoke(p, obj);
            }
            
            if (animateTile) StartCoroutine(AnimateTileMesh(true));
            
        }

        public void RandomizeRotation(Transform targetTransform) {
            Vector3 rot = targetTransform.rotation.eulerAngles;

            if (randomizeXRotation) rot.x = Random.Range(0, 360);
            if (randomizeYRotation) rot.y = Random.Range(0, 360);
            if (randomizeZRotation) rot.z = Random.Range(0, 360);

            targetTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(rot));
        }

        public Transform GetEndpointTransform() {
            return endPoint.transform;
        }

        public void OnDisable() {
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
