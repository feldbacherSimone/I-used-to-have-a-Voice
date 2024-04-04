using System;

namespace _IUTHAV.Core_Programming.Gamemode {
    
    /// <summary>
    /// Naming Convention:
    ///
    /// All states should have a Prefix followed by a '_', declaring in what scenes they are used in
    /// The letters should match the "StatePrefix" enum listed below
    /// Example: **PER_**SceneFinished_1
    ///
    /// States that fulfill the same roles, only in a different time, should have the same name.
    /// Differentiate between them using an integral Suffix
    /// Example: SC1_ACTFINISHED**_1**
    ///
    /// Not all States need a Suffix. If their behaviour is unique, ommit the suffix entirely
    /// Example: SC1_ReachedScrollTrigger
    ///
    /// This is used so states can be trimmed using '_' and their pre/suffixes can be changed
    /// </summary>
    [Serializable]
    public enum StateType {
        
        None,
        
        //Persistent States
        PER_State_1 = 1,
        
        //SCENE 1 States
        SC1_ReachedScrollTrigger = 100,
        SC1_ActFinished_0,
        SC1_ActFinished_1,
        SC1_ActFinished_2,

        //SCENE 2 States
        SC2_ReachedScrollEnd = 200,
        
        //SCENE 3 States
        SC3_ReachedScrollEnd = 300,
    
    }

    [Serializable]
    public enum StatePrefix {
        None,
        PER,
        SC1,
        SC2,
        SC3,
    }
}
