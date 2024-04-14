namespace _IUTHAV.Scripts.Dialogue {
    public enum ContinueMode {
        
        None,
        NeverWaitForContinue,
        AlwaysWaitForContinue,
        WaitUntilCharacterSwitch,
        WaitUntilPanelSwitch
        
    }

    public enum ContinueTiming {
        None,
        PreLine,
        PostLine,
    }
}