using System;

namespace _IUTHAV.Scripts.Core.Gamemode {
    
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
    /// Example: SC1_ScrollTrigger
    ///
    /// This is used so states can be trimmed using '_' and their pre/suffixes can be changed
    /// </summary>
    [Serializable]
    public enum StateType {
        
        None,
        
        //Persistent States
        PER_TheCarFinished,
        PER_TheTrainFinished,
        PER_TheMessageFinished,
        
        //SCENE 1 States
        SC1_ScrollTrigger = 100,
        SC1_Start = 101,
        SC1_Act1_Conv1,
        SC1_Act1_Conv2,
        SC1_Act1_Conv3,
        SC1_Act2_Conv1,
        SC1_Act2_Conv2,
        SC1_Act2_Conv3,
        SC1_Act3_Conv1,
        SC1_Act3_Conv2,
        SC1_Act3_Conv3,
        SC1_Act3_Conv4,

        SC1_SunCoverClicked = 140,
        SC1_CDGameStart,
        SC1_GloveBoxClicked,
        SC1_StartAct2,
        SC1_StartAct2_Conv2,
        SC1_StartAct2_Conv3,
        SC1_StartAct3_Conv3,
        SC1_EmPanic,

        //SCENE 2 States
        SC2_ScrollTrigger = 200,
        SC2_Start = 201,
        SC2_RingPhone,
        SC2_Act1_Conv1,
        SC2_Act1_Conv2,
        SC2_Act1_Conv3,
        SC2_Act2_Conv1,
        SC2_Act2_Conv2,
        SC2_Act2_Conv3,
        SC2_Act3_Conv1,
        SC2_Act3_Conv2,
        SC2_Act3_Conv3,
        SC2_Act3_Conv4,
        SC2_Act3_Conv5,
        SC2_Act4_Conv1,
        SC2_Act4_Conv2,
        
        SC2_StartAct1_Conv2 = 240,
        SC2_StartTrainEnter,
        SC2_StartAct2_Conv1,
        SC2_StartAct2_Conv2,
        SC2_StartAct3_Conv1,
        SC2_AllOptionsTried,
        SC2_StartAct4_Conv1,
        SC2_StartAct4_Conv2,
        //SCENE 3 States
        SC3_ScrollTrigger = 300,
    
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
