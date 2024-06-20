using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

namespace _IUTHAV.Scripts.Tilemap {
    public class TileManager : MonoBehaviour {

        [SerializeField] private TileController[] tileControllers;
        [SerializeField] private float speedChangeDuration = 2f;

#region Public Functions

        [YarnCommand("ChangeSpeedFactor")]
        public void ChangeSpeedFactor(float f) {

            foreach (TileController ctrl in tileControllers) {
            
                float target = Math.Clamp(f * ctrl.scrollSpeed, 0, 100);

                StartCoroutine(LerpTileSpeed(ctrl, target));

            }

        }
        
        [YarnCommand("ChangeSpeedAbs")]
        public void ChangeSpeedAbs(float f) {
            
            foreach (TileController ctrl in tileControllers) {
            
                float target = Math.Clamp(f + ctrl.scrollSpeed, 0, 100);

                StartCoroutine(LerpTileSpeed(ctrl, target));

            }
            
        }

        //[YarnCommand("ChangeSpeedAbs")]
        public void IncrementTileQueue(int i) {

            foreach (var ctrl in tileControllers) {
                
                ctrl.IncrementTileIndex(i);
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
