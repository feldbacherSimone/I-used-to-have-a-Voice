using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _IUTHAV.Scripts.Tilemap {
    public abstract class TileController : MonoBehaviour {
        
        [SerializeField] protected Transform controlPoint;
        [SerializeField] public float scrollSpeed = 2f;
        [SerializeField] private List<Tile> tiles;
        [SerializeField] protected TilelistType tileListType;
        [SerializeField] [Range(0,20)] protected int maxTiles = 5;

        public UnityEvent onLastTileReached;
        
        protected int _mCurrentTileIndex;
        protected Vector3 _mCurrentEndPoint;
        protected Queue<Tile> _mTiles;
        protected bool _mScrolling;
        
        private void Update() {
            
            if (_mScrolling) {
                
                UpdateTile();
            };
        }

#region Public Functions

        public void StartTileScroll() {

            _mScrolling = true;
        }

        public void StopTilescroll() {

            _mScrolling = false;
        }

        public void StopSpawningTiles() {

            maxTiles = 0;

        }

        public void QueueTile(Tile tile) {
            
            tiles.Add(tile);
        }

#endregion

#region Private Functions

        protected virtual void Configure(Vector3 endPoint) {
            _mCurrentEndPoint = endPoint;
            _mScrolling = true;
            _mTiles = new Queue<Tile>(GetComponentsInChildren<Tile>());
        }

        protected virtual void SpawnInstance() {

            Tile tile;

            if (tileListType == TilelistType.Queued) {

                tile = tiles[_mCurrentTileIndex];
                _mCurrentTileIndex++;
                if (_mCurrentTileIndex == tiles.Count) _mCurrentTileIndex = 0;
            }
            else {

                tile = tiles[Random.Range(0, tiles.Count - 1)];

            }
            
            var tileObj = Instantiate(tile.gameObject, this.gameObject.transform, true);
            tileObj.transform.localPosition = Vector3.zero;
            _mTiles.Enqueue(tileObj.GetComponent<Tile>());
            
        }

        protected virtual bool DespawnInstance() {

            if (_mTiles.TryDequeue(out Tile tile)) {
                
                tile.DestroyTile();
                Destroy(tile.gameObject);

            }

            if (_mTiles.Count == 0) {
                onLastTileReached.Invoke();
                _mScrolling = false;
                return false;
            }

            return true;
        }

        protected abstract void UpdateTile();

#endregion

#region Helper Classes

        protected enum TilelistType {
            random,
            Queued
        }
        

#endregion
        
    }
    
}
