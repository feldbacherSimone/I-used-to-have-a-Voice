using _IUTHAV.Scripts.Lerpables;
using UnityEngine.Playables;

namespace _IUTHAV.Scripts.Timeline.Lerpable {
    public class LerpableControlMixerBehaviour : PlayableBehaviour {
        
        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            ILerpable trackBinding = playerData as ILerpable;
            float finalInput = 0f;
            
            if (trackBinding == null)
                return;

            int inputCount = playable.GetInputCount (); //get the number of all clips on this track
            
            for (int i = 0; i < inputCount; i++) {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<LerpableControlBehaviour> inputPlayable = (ScriptPlayable<LerpableControlBehaviour>)playable.GetInput(i);
                LerpableControlBehaviour input = inputPlayable.GetBehaviour();

                // Use the above variables to process each frame of this playable.
                finalInput += input.CurrentLerpvalue * inputWeight;
            }
            
            //assign the result to the bound object
            trackBinding.SetLerpValue(finalInput);
        }
        
    }
}