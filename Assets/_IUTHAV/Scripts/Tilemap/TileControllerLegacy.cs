using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Tilemap {
    public abstract class TileControllerLegacy : MonoBehaviour {
        
        [SerializeField] public float scrollSpeed = 2f;
        [SerializeField] private List<Tile> tiles;
        [SerializeField] protected TileSwitchMode tileSwitchMode;
        [SerializeField] [Range(0,20)] protected int maxTiles = 5;
        [SerializeField] [Range(0,20)] protected int startingTiles = 3;

        public UnityEvent onLastTileReached;
        
        protected int _mCurrentTileIndex;
        protected Transform _mCurrentEndPoint;
        protected Transform _mControlPoint;
        protected Queue<Tile> _mTiles;
        
#region Public Functions
        
        public void QueueTile(Tile tile) {
            
            SpawnInstance(tile);
        }
        
        public void IncrementTileIndex(int i) {
            _mCurrentTileIndex += i;
            if (_mCurrentTileIndex == tiles.Count) _mCurrentTileIndex = 0;
        }
            
        public void NextTile() {
            IncrementTileIndex(1);
        }

#endregion

#region Private Functions

        protected void Configure() {
            
            _mTiles = new Queue<Tile>(GetComponentsInChildren<Tile>());
            
            _mControlPoint = Instantiate(new GameObject("CTRL"), transform).transform;
            _mCurrentEndPoint = Instantiate(new GameObject("END"), transform).transform;
            
            _mCurrentEndPoint.localPosition = _mTiles.Peek().GetEndpointTransform().localPosition;
            _mCurrentEndPoint.localRotation = _mTiles.Peek().GetEndpointTransform().localRotation;
            _mCurrentEndPoint.localScale = _mTiles.Peek().GetEndpointTransform().localScale;

            while (_mTiles.Count < startingTiles) {
                
                SpawnInstance();
            }
        }
        
        protected virtual void SpawnInstance(Tile tile = null) {
            
            if (tile == null) {
                if (tileSwitchMode == TileSwitchMode.Queued) {
                    
                    tile = tiles[_mCurrentTileIndex];
                    _mCurrentTileIndex++;
                    
                if (_mCurrentTileIndex == tiles.Count) _mCurrentTileIndex = 0;
                }
                else if (tileSwitchMode == TileSwitchMode.Random) {

                    tile = tiles[Random.Range(0, tiles.Count - 1)];

                }
                else {
                    tile = tiles[_mCurrentTileIndex];
                }
            }
            
            var tileObj = Instantiate(tile.gameObject, this.gameObject.transform, true);
            tileObj.transform.localPosition = Vector3.zero;
            _mTiles.Enqueue(tileObj.GetComponent<Tile>());

        }

        protected virtual bool DespawnInstance() {

            if (_mTiles.TryDequeue(out Tile tile)) {
                
                tile.OnDisable();
                Destroy(tile.gameObject);

            }

            if (_mTiles.Count == 0) {
                onLastTileReached.Invoke();
                return false;
            }

            return true;
        }

        protected abstract void UpdateTile();

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
