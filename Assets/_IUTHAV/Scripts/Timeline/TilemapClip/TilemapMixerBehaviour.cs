using _IUTHAV.Scripts.Lerpables;
using _IUTHAV.Scripts.Tilemap;
using UnityEngine.Playables;

namespace _IUTHAV.Scripts.Timeline.TilemapClip {
    public class TilemapMixerBehaviour : PlayableBehaviour {
        
        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            TileControllerLegacy trackBinding = playerData as TileControllerLegacy;
            float finalInput = 0f;
            
            if (trackBinding == null)
                return;

            int inputCount = playable.GetInputCount (); //get the number of all clips on this track
            
            for (int i = 0; i < inputCount; i++) {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<TilemapControlBehaviour> inputPlayable = (ScriptPlayable<TilemapControlBehaviour>)playable.GetInput(i);
                TilemapControlBehaviour input = inputPlayable.GetBehaviour();
                
                // Use the above variables to process each frame of this playable.
                finalInput += input.CurrentPosition * inputWeight;
            }
            
            //assign the result to the bound object
            //trackBinding
            playable.GetTime();
        }
        
    }
}