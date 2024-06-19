using UnityEngine;

namespace _IUTHAV.Scripts.Tilemap {
    public class TilePositionController : TileController {

        [SerializeField] private Transform targetEndpoint;

#region Unity Functions

        private void Awake() {

            Configure(targetEndpoint.localPosition);
        }

#endregion

#region Private Functions

        protected override void SpawnInstance() {

            base.SpawnInstance();
            
            if (_mTiles.Peek().GetLocalPosEndpoint() != _mCurrentEndPoint) {
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

            }

            return base.DespawnInstance();
        }

        private void UpdateEndpoint(Tile tile) {

            _mCurrentEndPoint = tile.GetLocalPosEndpoint();
        }

        protected override void UpdateTile() {
        
            controlPoint.localPosition = Vector3.MoveTowards(controlPoint.localPosition,
             Vector3.zero,
             scrollSpeed * Time.deltaTime);

             if (controlPoint.localPosition == Vector3.zero) {
                if (_mTiles.Count <= maxTiles) {
                    SpawnInstance();
                }

                if (_mTiles.Count > maxTiles) {
                    if (!DespawnInstance()) return;
                }
                controlPoint.localPosition = _mCurrentEndPoint;
             }

             var t = _mTiles.ToArray();
             
             t[^1].gameObject.transform.localPosition = controlPoint.localPosition;
        }

#endregion
        
    }
}
