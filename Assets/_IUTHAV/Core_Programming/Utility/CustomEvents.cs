using System;
using _IUTHAV.Core_Programming.Gamemode;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _IUTHAV.Core_Programming.Utility {
    
    [Serializable]
    public class GameStateDataEvent : UnityEvent<IFinishable, StateType> {}

    public class GameStateEvent : UnityEvent<StateType> { }

    public class StateDataEventArgs : EventArgs { public IFinishable Finishable; }

}
