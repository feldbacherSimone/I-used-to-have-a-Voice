using System;
using UnityEngine;

namespace _IUTHAV.Scripts.Panic {
    
    /// <summary>
    /// This interface is a basic skeleton for IPanicables.
    /// It requires the class, which implements it, to keep
    /// track of it's local states themselves. this includes
    /// 5 variables:
    /// 
    /// minParameterValue
    /// maxParameterValue
    /// _baseParameterValue
    /// _targetParameterValue
    /// _currentParameterValue
    ///
    /// the abstract class below: PanicParameter, implements these
    /// values with a type T and can be used to ease programming
    /// Existing scripts, which should be made Panicable, must
    /// implement the values mentioned above on their own
    /// </summary>
    public interface IPanicable {

        /// <summary>
        /// Called by PanicController right after "LerpByPanicDelta" is called
        /// Expects the User to set the desired Parametervalue like an Objects
        /// value T to the local variable T _currentParameterValue
        /// </summary>
        public void SetDesiredParameter();
        /// <summary>
        /// Is called right before the loop for changing all
        /// Parameters is started by the PanicController
        /// Expects the User to set the STARTING value T for
        /// the gameobject in question and save it to a local
        /// variable T _baseParametervalue
        /// </summary>
        public void SetBaseParameter();
        /// <summary>
        /// Very similar to SetBaseParameter
        /// Is called right before the loop for changing all
        /// Parameters is started by the PanicController
        /// Expects the User to set the TARGET value T for
        /// the gameobject in question and save it to a local
        /// variable T _baseParametervalue
        ///
        /// It expects Users to do this by using a
        /// local Variable minPanicParameter and
        /// maxPanicParameter and the float targetValue
        /// (float between 0-1) to call a Lerp function
        /// </summary>
        public void SetTargetParameter(float targetValue);
        
        /// <summary>
        /// The function, which lerps a variable T,
        /// should be defined here and set a local
        /// variable T _currentParameterValue to
        /// it's appropriate state.
        ///
        /// It should use the same lerp function as
        /// SetTargetParameter, only this time
        /// using _baseParametervalue as a starting point
        /// using _targetParametervalue as a target
        /// using targetValue as the t
        /// setting a local variable _currentParameterValue
        /// to the result of the lerp
        /// </summary>
        /// <param name="targetValue"></param>
        public void LerpByPanicValue(float targetValue);
        
    }

    [Serializable]
    public abstract class PanicParameter<T> : MonoBehaviour, IPanicable {
        
        /// <summary>
        /// State when Panic = 0
        /// </summary>
        [SerializeField] protected T minPanicParameter;
        /// <summary>
        /// State when Panic = 1
        /// </summary>
        [SerializeField] protected T maxPanicParameter;
      
        protected T _currentParameterValue;
        /// <summary>
        /// Buffer variable, to use for Lerp-Function BaseValue
        /// </summary>
        protected T _baseParameterValue;
        /// <summary>
        /// Buffer variable, to use for Lerp-Function TargetValue
        /// </summary>
        protected T _targetParameterValue;
        
        public abstract void SetDesiredParameter();
        public abstract void SetBaseParameter();
        public abstract void SetTargetParameter(float targetValue);
        public abstract void LerpByPanicValue(float targetValue);


    }
}
