using _IUTHAV.Scripts.CustomUI;
using UnityEngine.Playables;

namespace _IUTHAV.Scripts.Timeline.ScrollBackground {
    public class ScrollBackgroundControlMixerBehaviour : PlayableBehaviour {
        
        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            ScrollBackGround trackBinding = playerData as ScrollBackGround;
            float finalScrollPosition = 0f;

            if (!trackBinding)
                return;

            int inputCount = playable.GetInputCount (); //get the number of all clips on this track

            for (int i = 0; i < inputCount; i++) {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<ScrollBackgroundControlBehaviour> inputPlayable = (ScriptPlayable<ScrollBackgroundControlBehaviour>)playable.GetInput(i);
                ScrollBackgroundControlBehaviour input = inputPlayable.GetBehaviour();

                // Use the above variables to process each frame of this playable.
                finalScrollPosition += input.ScrollPosition * inputWeight;
            }

            //assign the result to the bound object
            trackBinding.ScrollPosition = finalScrollPosition;
        }
        
    }
}