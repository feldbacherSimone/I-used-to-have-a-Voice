using System;

namespace _IUTHAV.Core_Programming.Gamemode {

    [Serializable]
    public enum StateType {
        
        None,
        
        //Persistent States
        PER_State1 = 1,
        
        //SCENE 1 States
        SC1_State1 = 100,
        SC1_State2,
        SC1_State3,
        SC1_State4,

        //SCENE 2 States
        SC2_State1 = 200,
        
        //SCENE 3 States
        SC3_State1 = 300,
    
    }
}
