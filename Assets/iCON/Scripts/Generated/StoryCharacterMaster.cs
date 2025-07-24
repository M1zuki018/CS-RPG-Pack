// ============================================================================
// AUTO GENERATED - DO NOT MODIFY
// Generated at: 2025-07-24 12:43:20
// ============================================================================

using System.Collections.Generic;
using UnityEngine;
using iCON.System;

/// <summary>
/// ストーリーキャラクター情報の定数クラス
/// </summary>
public static class StoryCharacterMaster
{
    private static readonly Dictionary<int, CharacterData> _characterData = new Dictionary<int, CharacterData>
    {
        {
            0, new CharacterData(0, "黒華琴葉", "琴葉", 
                new Color(0.545f, 0.000f, 0.000f, 1.000f), 0.06f,
                new Dictionary<FacialExpressionType, string>
                {
                    { FacialExpressionType.Default, "Assets/AssetStoreTools/Images/Characters/kotoha/kotoha_Default.png" },
                    { FacialExpressionType.Nervous, "Assets/AssetStoreTools/Images/Characters/kotoha/kotoha_Nervous.png" },
                    { FacialExpressionType.Sigh, "Assets/AssetStoreTools/Images/Characters/kotoha/kotoha_Sigh.png" },
                    { FacialExpressionType.Surprised, "Assets/AssetStoreTools/Images/Characters/kotoha/kotoha_Surprised.png" },
                    { FacialExpressionType.Smile, "Assets/AssetStoreTools/Images/Characters/kotoha/kotoha_Smile.png" },
                    { FacialExpressionType.Blush, "Assets/AssetStoreTools/Images/Characters/kotoha/kotoha_Blush.png" },
                    { FacialExpressionType.Embarrassed, "Assets/AssetStoreTools/Images/Characters/kotoha/kotoha_Embarrassed.png" },
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
