using System;
using _IUTHAV.Core_Programming.Gamemode;
using _IUTHAV.Core_Programming.Scene;
using UnityEngine.Events;

namespace _IUTHAV.Core_Programming.Utility {
    [Serializable]
    public class SceneEvent : UnityEvent <SceneLoadParameters> {}
    
    [Serializable]
    public class GameStateEvent : UnityEvent<StateData> {}

}
