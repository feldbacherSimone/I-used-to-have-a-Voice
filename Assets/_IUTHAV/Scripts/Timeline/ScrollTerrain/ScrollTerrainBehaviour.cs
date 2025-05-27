using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Timeline.ScrollTerrain {

    [Serializable]
    public class ScrollTerrainBehaviour : PlayableBehaviour {
        
        //Unity playables don't support complex types if used this way - need to find a workaround
        //[field:SerializeField] private Vector2 positionOffset;
        [SerializeField] private float speed = 1f;
        [HideInInspector] public float SpeedWeight;
        
        [SerializeField] private Texture2D heightMap;
        public Texture2D HeightMap => heightMap;

        private float previousTime;
        private float currentTime;
        private float currentOffset;
        
        private const string OffsetValueName = "_TilingOffset";
        private const string HeightMapNameA = "_HeightMap_A";
        private const string HeightMapNameB = "_HeightMap_B";
        private const string BlendValueNameA = "_HeightMap_A_Blend";
        private const string BlendValueNameB = "_HeightMap_B_Blend";

        private string assignedMap;

        private Material material;

        private bool firstFrameProcessed;
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            
            material = playerData as Material;
            if (material == null || heightMap == null) {
                return;
            }

            if (!firstFrameProcessed) {
                firstFrameProcessed = true;
                if (material.GetTexture(HeightMapNameA) == null) {
                    material.SetTexture(HeightMapNameA, heightMap);
                    material.SetFloat(BlendValueNameA, 1f);
                    material.SetFloat(BlendValueNameB, 0f);
                }
                else if (material.GetFloat(BlendValueNameA) > material.GetFloat(BlendValueNameB)) {
                    material.SetTexture(HeightMapNameB, heightMap);
                    assignedMap = HeightMapNameB;
                }
                else {
                    material.SetTexture(HeightMapNameA, heightMap);
                    assignedMap = HeightMapNameA;
                }
            }

            previousTime = currentTime;
            currentTime = (float)playable.GetTime();
            currentOffset = material.GetVector(OffsetValueName).y;
            
            //Normalize the offset so texture doesn't scroll indefinitly
            var maxOffset = 1;
            if (heightMap.wrapMode.Equals(TextureWrapMode.Mirror)) {
                maxOffset = 2;
            }
            currentOffset = ((currentOffset + maxOffset) % (2 * maxOffset)) - maxOffset;

            float deltaTime = currentTime - previousTime;
            float offset = currentOffset + deltaTime * speed * SpeedWeight;
            
            material.SetVector(OffsetValueName, new Vector2(0, offset));
        }


        public override void OnBehaviourPause(Playable playable, FrameData info) {

            firstFrameProcessed = false;
            if (!string.IsNullOrEmpty(assignedMap)) material.SetTexture(assignedMap, null);
            
            base.OnBehaviourPause(playable, info);
        }
    }
}