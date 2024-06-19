using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Tilemap {
    public class TileSpeedController : MonoBehaviour {

        [SerializeField] private TileController[] tileControllers;
        [SerializeField] private float speedChangeDuration = 2f;

#region Public Functions

        public void ChangeSpeedFactor(float f) {

            foreach (TileController ctrl in tileControllers) {
            
                float target = Math.Clamp(f * ctrl.scrollSpeed, 0, 100);

                StartCoroutine(LerpTileSpeed(ctrl, target));

            }

        }

        public void ChangeSpeedAbs(float f) {
            
            foreach (TileController ctrl in tileControllers) {
            
                float target = Math.Clamp(f + ctrl.scrollSpeed, 0, 100);

                StartCoroutine(LerpTileSpeed(ctrl, target));

            }
            
        }

#endregion

        private IEnumerator LerpTileSpeed(TileController tile, float targetSpeed) {

            float t = 0;
            float startSpeed = tile.scrollSpeed;

            while (t < speedChangeDuration) {

                tile.scrollSpeed = Mathf.Lerp(startSpeed, targetSpeed, t / speedChangeDuration);
                
                t += Time.deltaTime;
                yield return null;
            }

        }

    }
}
