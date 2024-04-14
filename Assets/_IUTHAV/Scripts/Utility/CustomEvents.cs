using System;
using _IUTHAV.Scripts.Core.Gamemode;
using UnityEngine.Events;

namespace _IUTHAV.Scripts.Utility {
    
    [Serializable]
    public class GameStateDataEvent : UnityEvent<IFinishable, StateType> {}

    public class GameStateEvent : UnityEvent<StateType> { }

    public class StateDataEventArgs : EventArgs { public IFinishable finishable; }

}
