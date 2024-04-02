using System;
using _IUTHAV.Core_Programming.Gamemode;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _IUTHAV.Core_Programming.Utility {
    
    [Serializable]
    public class GameStateEvent : UnityEvent<IFinishable> {}

    public class StateDataEventArgs : EventArgs {
        public IFinishable finishable;
    }

}
