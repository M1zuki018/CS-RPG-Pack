// ============================================================================
// AUTO GENERATED - DO NOT MODIFY
// Generated at: 2025-07-28 23:37:51
// ============================================================================

using System.Collections.Generic;
using UnityEngine;
using iCON.System;

/// <summary>
/// ストーリーキャラクター情報の定数クラス
/// </summary>
public static class MasterStoryScene
{
    private static readonly Dictionary<int, CharacterData> _characterData = new Dictionary<int, CharacterData>
    {
        {
            1, new CharacterData(1, "琴葉の休息", "1", 
                Color.white, 1.00f,
                new Dictionary<FacialExpressionType, string>
                {
                    { FacialExpressionType.Default, "A3:O218" },
                    { FacialExpressionType.Nervous, "1" },
                    { FacialExpressionType.Sigh, "0-0-0" },
                })
        },
        {
            2, new CharacterData(2, "マキ・幼少期 - 1", "2", 
                Color.white, 1.00f,
                new Dictionary<FacialExpressionType, string>
                {
                    { FacialExpressionType.Default, "A220:O298" },
                    { FacialExpressionType.Nervous, "1.2" },
                    { FacialExpressionType.Sigh, "180-60-0" },
                })
        },
        {
            3, new CharacterData(3, "マキ・幼少期 - 2", "2", 
                Color.white, 2.00f,
                new Dictionary<FacialExpressionType, string>
                {
                    { FacialExpressionType.Default, "A300:O359" },
                    { FacialExpressionType.Nervous, "1.2" },
                    { FacialExpressionType.Sigh, "180-60-0" },
                    { FacialExpressionType.Surprised, "2" },
                })
        },
    };

    /// <summary>
    /// IDからキャラクターデータを取得
    /// </summary>
    public static CharacterData GetCharacter(int id)
    {
        return _characterData.GetValueOrDefault(id, null);
    }

    /// <summary>
    /// フルネームからキャラクターデータを取得
    /// </summary>
    public static CharacterData GetCharacterByName(string fullName)
    {
        foreach (var kvp in _characterData)
        {
            if (kvp.Value.FullName == fullName)
                return kvp.Value;
        }
        return null;
    }

    /// <summary>
    /// キャラクターの表情パスを取得
    /// </summary>
    public static string GetExpressionPath(int characterId, FacialExpressionType expression)
    {
        var character = GetCharacter(characterId);
        if (character?.ExpressionPaths?.ContainsKey(expression) == true)
        {
            return character.ExpressionPaths[expression];
        }
        return null;
    }

    /// <summary>
    /// 全キャラクターのIDリストを取得
    /// </summary>
    public static IEnumerable<int> GetAllCharacterIds()
    {
        return _characterData.Keys;
    }

    /// <summary>
    /// 全キャラクターデータを取得
    /// </summary>
    public static IEnumerable<CharacterData> GetAllCharacters()
    {
        return _characterData.Values;
    }
}
