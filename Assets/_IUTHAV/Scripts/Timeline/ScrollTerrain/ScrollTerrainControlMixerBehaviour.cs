using System;
using UnityEngine;
using UnityEngine.Playables;

namespace _IUTHAV.Scripts.Timeline.ScrollTerrain {

    
    public class ScrollTerrainControlMixerBehaviour : PlayableBehaviour {

        private Material material;

        private bool firstFrameProcessed;

        private int currentMainClipIndex;

        private bool blending;
        private bool blendStart;

        private const string BlendValueNameA = "_HeightMap_A_Blend";
        private const string BlendValueNameB = "_HeightMap_B_Blend";
        private string mainTextureFieldName;
        private string secondaryTextureFieldName;
        private const string HeightMapName_A = "_HeightMap_A";
        private const string HeightMapName_B = "_HeightMap_B";

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {

            material = playerData as Material;

            if (material == null) {
                return;
            }

            if (!firstFrameProcessed) {
                firstFrameProcessed = true;
                
            }

            int playableCount = playable.GetInputCount();

            float totalWeigth = 0f;

            for (int i = 0; i < playableCount; i++) {
                 float playableWeight = playable.GetInputWeight(i);
                 
                totalWeigth += playableWeight;

                 ScriptPlayable<ScrollTerrainBehaviour> scriptPlayable = (ScriptPlayable<ScrollTerrainBehaviour>)playable.GetInput(i);
                 ScrollTerrainBehaviour behaviour = scriptPlayable.GetBehaviour();
                 behaviour.SpeedWeight = playableWeight;
                 
                 if (playableWeight == 0f) {
                     continue;
                 }
                 string textureFieldName = i % 2 == 0 ? BlendValueNameA : BlendValueNameB;
                 material.SetFloat(textureFieldName, playableWeight);
            }
            
            //reset blend values if no clip is active
            if (totalWeigth == 0f) {
                material.SetFloat(BlendValueNameA, 1f);
                material.SetFloat(BlendValueNameB, 0f);
                material.SetTexture(HeightMapName_A, null);
                material.SetTexture(HeightMapName_B, null);
            }
        }
        
        public override void OnBehaviourPause(Playable playable, FrameData info) {

            firstFrameProcessed = false;
            
            base.OnBehaviourPause(playable, info);
        }

        private ScrollTerrainBehaviour GetBehaviourClip(int inputIndex, Playable playable) {
            ScrollTerrainBehaviour behaviour = null;
            
            ScriptPlayable<ScrollTerrainBehaviour> inputPlayable =
                (ScriptPlayable<ScrollTerrainBehaviour>)playable.GetInput(inputIndex);

            behaviour = inputPlayable.GetBehaviour();

            return behaviour;
        }
            
    }
}