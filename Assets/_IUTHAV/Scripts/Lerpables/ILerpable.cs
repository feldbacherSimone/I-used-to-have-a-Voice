using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Lerpables {
    
    public interface ILerpable {
        public void SetLerpValue(float targetValue);
        
    }
    
    public abstract class LerpParameter<T> : MonoBehaviour, ILerpable {
        
        [SerializeField] 
        [Tooltip("State when Panic = 0")] 
        protected T minLerpParam;
        
        [SerializeField]
        [Tooltip("State when Panic = 1")] 
        protected T maxLerpParam;
        
        protected T _currentParameterValue;
        
        public abstract void SetLerpValue(float targetValue);


    }
}
