namespace CryStar.Story.Enums
{
    /// <summary>
    /// スプレッドシートの列定義の列挙型
    /// intに対応する列を読み込むため、スプレッドシートの列構成を元にintの値も設定
    /// </summary>
    public enum StoryDataColumnType
    {
        PartId = 0,
        ChapterId = 1,
        SceneId = 2,
        OrderId = 3,
        OrderType = 4,
        Sequence = 5,
        SpeakerId = 6,
        DialogText = 7,
        OverrideDisplayName = 8,
        CharacterPosition = 10,
        FacialExpression = 11,
        OverrideTextSpeed = 12,
        Duration = 13,
        FilePath = 14,
    }
}