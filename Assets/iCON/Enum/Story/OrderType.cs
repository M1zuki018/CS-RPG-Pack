namespace iCON.Enums
{
    /// <summary>
    /// ストーリーのオーダーの列挙型
    /// </summary>
    public enum OrderType 
    {
        Start = 0,
        Talk = 1,
        Descriptive = 2,
        End = 3,
        ChangeBGM = 4,
        CharacterEntry = 5,
        CharacterChange = 6,
        CharacterExit = 7,
        ShowSteel = 8,
        HideSteel = 9,
        CameraShake = 10,
        Choice = 11,
        Effect = 12,
        ChangeBackground = 13,
        Wait = 14,
        ChangeLighting = 15,
        
        Custom = 99,
    }
}

