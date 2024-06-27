using UnityEngine;

namespace _IUTHAV.Scripts.Tilemap {
    public class TileRotationController : TileController {

#region Unity Functions

        private void Awake() {

            Configure();
        }

#endregion

#region Private Functions

        protected override void UpdateTile() {
        
            _mControlPoint.localRotation = Quaternion.RotateTowards(_mControlPoint.localRotation,
             _mCurrentEndPoint.localRotation,
             scrollSpeed * Time.deltaTime);
            
             if (_mControlPoint.localRotation == _mCurrentEndPoint.localRotation) {
                if (_mTiles.Count <= maxTiles && maxTiles != 0) {
                    SpawnInstance();
                }

                if (_mTiles.Count > maxTiles) {
                    
                    if (!DespawnInstance()) return;
                }
                _mControlPoint.localRotation = Quaternion.identity;
             }

             var t = _mTiles.ToArray();
             
             var targetRotation = _mControlPoint.localRotation.eulerAngles;
             for (int i = 0, j = t.Length-1; i < t.Length; i++, j--) {
                 Vector3 rot = _mCurrentEndPoint.localRotation.eulerAngles * i + targetRotation;
                 t[j].transform.localRotation = Quaternion.Euler(rot);
             }
        }

#endregion
        
    }
}
