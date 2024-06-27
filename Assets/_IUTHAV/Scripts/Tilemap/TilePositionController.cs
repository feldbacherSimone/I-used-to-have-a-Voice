using UnityEngine;

namespace _IUTHAV.Scripts.Tilemap {
    public class TilePositionController : TileController {

        [SerializeField] private Transform targetEndpoint;

#region Unity Functions

        private void Awake() {

            Configure();
        }

#endregion

#region Private Functions

        protected override void SpawnInstance(Tile tile = null) {

            base.SpawnInstance();
            
            if (_mTiles.Peek().GetEndpointTransform() != _mCurrentEndPoint) {
                UpdateEndpoint(_mTiles.Peek());
            }
            
            var t = _mTiles.ToArray();
            
            if (_mTiles.Count > 1) {
            
                t[_mTiles.Count-2].transform.SetParent(t[^1].gameObject.GetComponent<Tile>().EndPoint.transform);
                t[_mTiles.Count-2].transform.localPosition = Vector3.zero;
                t[_mTiles.Count-2].transform.rotation = Quaternion.identity;
            }
            
        }

        protected override bool DespawnInstance() {
            
            if (_mTiles.TryDequeue(out Tile tile)) {
                
                tile.EndPoint.transform.DetachChildren();
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

        private void UpdateEndpoint(Tile tile) {

            _mCurrentEndPoint = tile.GetEndpointTransform();
        }

        protected override void UpdateTile() {
        
            _mControlPoint.localPosition = Vector3.MoveTowards(_mControlPoint.localPosition,
             Vector3.zero,
             scrollSpeed * Time.deltaTime);

             if (_mControlPoint.localPosition == Vector3.zero) {
                if (_mTiles.Count <= maxTiles) {
                    SpawnInstance();
                }

                if (_mTiles.Count > maxTiles) {
                    if (!DespawnInstance()) return;
                }
                _mControlPoint.localPosition = _mCurrentEndPoint.localPosition;
             }

             var t = _mTiles.ToArray();
             
             t[^1].gameObject.transform.localPosition = _mControlPoint.localPosition;
        }

#endregion
        
    }
}
