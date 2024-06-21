namespace _IUTHAV.Scripts.Dialogue {
    public enum ContinueMode {
        
        None,
        NeverWaitForContinue,
        AlwaysWaitForContinue,
        WaitUntilCharacterSwitch,
        WaitUntilPanelSwitch
        
    }

    public enum ContinueButtonTiming {
        None,
        PreLine,
        PostLine,
    }
}