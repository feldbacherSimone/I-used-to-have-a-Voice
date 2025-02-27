using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace _IUTHAV.Scripts.Tilemap {
    public class TileController : MonoBehaviour {

        [SerializeField] private float scrollSpeed = 2f;
        [SerializeField] private List<Tile> tiles = new List<Tile>();
        [SerializeField] private TileSwitchMode tileSwitchMode;
        [SerializeField] [Range(0, 20)] protected int maxTiles = 5;
        [SerializeField] [Range(0, 20)] protected int startingTiles = 3;

        public UnityEvent onLastTileReached;

        private int m_currentTileIndex;
        private Transform m_currentEndPoint;
        private Transform m_controlPoint;
        private Queue<Tile> m_tiles;
        
        private Dictionary<Tile, ObjectPool<GameObject>> m_tilePools;
        private Dictionary<GameObject, ObjectPool<GameObject>> m_spawnedObjectPools;
        
        [ContextMenu("Configure Object pools")]
        public void Configure() {

            m_tilePools = new Dictionary<Tile, ObjectPool<GameObject>>();
            
            m_spawnedObjectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
            
            Dictionary<GameObject, int> spawnObjects = new Dictionary<GameObject, int>();
            
            for (int i = 0; i < tiles.Count; i++) {
                m_currentTileIndex = i;
                m_tilePools.Add(tiles[i], GetPool(CreateTileObject, startingTiles, maxTiles));

                var objects = tiles[i].SpawnObjects;
                foreach (var obj in objects) {
                    if (spawnObjects.ContainsKey(obj)) {
                        spawnObjects[obj]++;
                    }
                    else {
                        spawnObjects.Add(obj, 1);
                    }
                }
            }

            ConfigureTileSpawnPointObjects(spawnObjects);

        }
        
        public void IncrementTileIndex(int i) {
            m_currentTileIndex += i;
            if (m_currentTileIndex == tiles.Count) m_currentTileIndex = 0;
        }

        private void ConfigureTileSpawnPointObjects(Dictionary<GameObject, int> spawnObjects) {
            foreach (var obj in spawnObjects) {
                m_spawnedObjectPools.Add(obj.Key, GetPool(CreateSpawnObject, obj.Value, obj.Value*2));
            }
            m_currentTileIndex = 0;
            
            foreach (var tile in tiles) {
                tile.OnRequestSpawnObjectChild += (spawnPoint, randomObject) => {
                    if (m_spawnedObjectPools.TryGetValue(randomObject, out var value)) {
                        var obj = value.Get();
                        obj.transform.SetParent(spawnPoint, true);
                        tile.RandomizeRotation(obj.transform);
                    }
                };
            }
        }
        
        private void SpawnTile(Tile tile = null) {
            
            if (tile == null) {
                if (tileSwitchMode == TileSwitchMode.Queued) {
                    
                    tile = tiles[m_currentTileIndex];
                    m_currentTileIndex++;
                    
                if (m_currentTileIndex == tiles.Count) m_currentTileIndex = 0;
                }
                else if (tileSwitchMode == TileSwitchMode.Random) {

                    tile = tiles[Random.Range(0, tiles.Count - 1)];

                }
                else {
                    tile = tiles[m_currentTileIndex];
                }
            }
            
            var tileObj = m_tilePools[tile].Get();
            tileObj.transform.SetParent(gameObject.transform, true);
            tileObj.transform.localPosition = Vector3.zero;
            m_tiles.Enqueue(tileObj.GetComponent<Tile>());

        }

        private bool DespawnTile() {

            if (m_tiles.TryDequeue(out Tile tile)) {
                tile.OnDisable();
                Destroy(tile.gameObject);
            }

            if (m_tiles.Count == 0) {
                onLastTileReached.Invoke();
                return false;
            }

            return true;
        }

#region Object Pool Creation

        private ObjectPool<GameObject> GetPool(Func<GameObject> creationDelegate, int defaultCapacity, int maxCapacity) {
            var pool = new ObjectPool<GameObject>(
                    creationDelegate,
                        OnGetFromPool,
                    OnReturnFromPool,
                    OnDestroyPooledObject,
                    true,
                    defaultCapacity,
                    maxCapacity);
            return pool;
        }

        private GameObject CreateSpawnObject() {
            var spawnObjects = tiles[m_currentTileIndex].SpawnObjects;
            int random = Random.Range(0, spawnObjects.Length);
            GameObject newObject = Instantiate(spawnObjects[random]);
            return newObject;
        }

        private GameObject CreateTileObject() {
            GameObject newTile = GameObject.Instantiate(tiles[m_currentTileIndex].gameObject);
            return newTile;
        }

        private void OnGetFromPool(GameObject pooledObject) => pooledObject.SetActive(true);
        private void OnReturnFromPool(GameObject pooledObject) {
            //pooledObject.transform.SetParent(gameObject.transform, true);
            pooledObject.SetActive(false);
        }

        private void OnDestroyPooledObject(GameObject pooledObject) => GameObject.Destroy(pooledObject);

#endregion
        
#region Helper Classes

        protected enum TileSwitchMode {
            Random,
            Queued,
            Manual
        }
        

#endregion
    }
    
    
}