// ============================================================================
// AUTO GENERATED - DO NOT MODIFY
// Generated at: 2025-07-15 00:29:49
// ============================================================================

using System.Collections.Generic;

/// <summary>
/// ワーディングキー定数クラス
/// </summary>
public static class WordingMaster
{
    private static readonly Dictionary<string, string> _data = new Dictionary<string, string>
    {
        { "BOOT_START", "Start" },
        { "LOADING", "Loading..." },
        { "STORY_IMMERSED", "UI\n非表示" },
        { "STORY_AUTO", "オート\n再生" },
        { "STORY_SKIP", "スキップ" },
        { "BATTLE_COMMAND_ATTCK", "アタック" },
        { "BATTLE_COMMAND_IDEA", "idea" },
        { "BATTLE_COMMAND_ITEM", "item" },
        { "BATTLE_COMMAND_GUARD", "まもる" },
        { "BATTLE_FIRSTSELECT_START", "たたかう" },
        { "BATTLE_FIRSTSELECT_ESCAPE", "にげる" },
        { "BATTLE_IDEA_COMMAND", "command" },
        { "BATTLE_IDEA_ACTOR", "actor" },
        { "BATTLE_FAILED_ESCAPE", "▼逃げ場などない" },
    };

    public static string GetText(string key)
    {
        return _data.GetValueOrDefault(key, null);
    }
}
