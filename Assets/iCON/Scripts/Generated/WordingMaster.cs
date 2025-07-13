// ============================================================================
// AUTO GENERATED - DO NOT MODIFY
// Generated at: 2025-07-13 15:05:19
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
    };

    public static string GetText(string key)
    {
        return _data.GetValueOrDefault(key, null);
    }
}
