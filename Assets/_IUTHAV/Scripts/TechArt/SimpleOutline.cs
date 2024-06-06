using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace _IUTHAV.Scripts.TechArt
{
    [VolumeComponentMenu("Custom/Simple Outline")]
    public class SimpleOutline : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter weight = new ClampedFloatParameter(value: 0, min: 0, max: 100);
        public NoInterpColorParameter lineColor = new NoInterpColorParameter(Color.black);
        public bool IsActive()
        {
            throw new System.NotImplementedException();
        }
    }
}
