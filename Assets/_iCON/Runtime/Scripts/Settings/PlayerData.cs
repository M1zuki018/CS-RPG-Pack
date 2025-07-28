using R3;

/// <summary>
/// プレイヤー情報を保持しておくための静的クラス
/// </summary>
public static class PlayerData
{
    public static ReactiveProperty<string> NameProp = new ReactiveProperty<string>("Default");
    public static ReactiveProperty<int> LevelProp = new ReactiveProperty<int>(1);

    
    /// <summary>
    /// 名前を設定する
    /// </summary>
    public static void SetName(string name) => NameProp.Value = name;
    
    /// <summary>
    /// レベルを設定する
    /// </summary>
    public static void SetLevel(int value) => LevelProp.Value = value;
    
    /// <summary>
    /// レベルアップ
    /// </summary>
    public static void LevelUp() => LevelProp.Value++;
    
    /// <summary>
    /// レベルリセット
    /// </summary>
    public static void LevelReset() => LevelProp.Value = 1;
}