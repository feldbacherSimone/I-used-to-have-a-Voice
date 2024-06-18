using UnityEngine;

namespace _IUTHAV.Scripts.Tilemap {
    public class TileRotationController : TileController {
        
        [SerializeField] private float targetXRotation;
        [SerializeField] private float targetYRotation;
        [SerializeField] private float targetZRotation;

#region Unity Functions

        private void Awake() {

            Configure(new Vector3(targetXRotation, targetYRotation, targetZRotation));
        }

#endregion

#region Private Functions

        protected override void UpdateTile() {
        
            controlPoint.localRotation = Quaternion.RotateTowards(controlPoint.localRotation,
             Quaternion.Euler(_mCurrentEndPoint),
             scrollSpeed * Time.deltaTime);

             if (controlPoint.localRotation == Quaternion.Euler(_mCurrentEndPoint)) {
                if (_mTiles.Count <= maxTiles) {
                    SpawnInstance();
                }

                if (_mTiles.Count > maxTiles) {
                    if (!DespawnInstance()) return;
                }
                controlPoint.localRotation = Quaternion.identity;
             }

             var t = _mTiles.ToArray();
             
             var targetRotation = controlPoint.localRotation.eulerAngles;
             for (int i = 0, j = t.Length-1; i < t.Length; i++, j--) {
                 Vector3 rot = _mCurrentEndPoint * i + targetRotation;
                 t[j].transform.localRotation = Quaternion.Euler(rot);
             }
        }

#endregion
        
    }
}
