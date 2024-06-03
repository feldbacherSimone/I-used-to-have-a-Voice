using System;
using UnityEngine;

namespace _IUTHAV.Scripts.Panic {
    
    public interface IPanicable {

        public void ChangeComponentParameter();
        public void SetBaseParameter();
        public void SetTargetParameter(float targetValue);
        public void LerpByPanicDelta(float targetValue);
        
    }

    [Serializable]
    public abstract class PanicParameter<T> : MonoBehaviour, IPanicable {
        
        [SerializeField] protected T minPanicParameter;
        [SerializeField] protected T maxPanicParameter;
        protected T _currentParameterValue;
        protected T _baseParameterValue;
        protected T _targetParameterValue;

        /// <summary>
        /// Use this function to set all wanted Components to the desired value
        /// </summary>
        public abstract void ChangeComponentParameter();
        public abstract void SetBaseParameter();
        public abstract void SetTargetParameter(float targetValue);
        
        /// <summary>
        /// Define, how the parameter T should be lerped
        /// </summary>
        /// <param name="targetValue">Value between 0-1, which represents the state of panic</param>
        /// <returns></returns>
        public abstract void LerpByPanicDelta(float targetValue);


    }
}
