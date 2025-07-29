// ============================================================================
// AUTO GENERATED - DO NOT MODIFY
// Generated at: 2025-07-25 16:27:56
// ============================================================================

using System.Collections.Generic;
using CryStar.Story.Data;
using CryStar.Story.Enums;
using UnityEngine;

/// <summary>
/// ストーリーキャラクター情報の定数クラス
/// </summary>
public static class MasterStoryCharacter
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
        {
            1, new CharacterData(1, "マキ少年時代", "マキ", 
                new Color(0.863f, 0.078f, 0.235f, 1.000f), 0.07f,
                new Dictionary<FacialExpressionType, string>
                {
                    { FacialExpressionType.Default, "Assets/AssetStoreTools/Images/Characters/maqi_s/maqi_s_Default.png" },
                    { FacialExpressionType.Nervous, "Assets/AssetStoreTools/Images/Characters/maqi_s/maqi_s_Nervous.png" },
                    { FacialExpressionType.Surprised, "Assets/AssetStoreTools/Images/Characters/maqi_s/maqi_s_Surprised.png" },
                    { FacialExpressionType.Smile, "Assets/AssetStoreTools/Images/Characters/maqi_s/maqi_s_Smile.png" },
                    { FacialExpressionType.Determination, "Assets/AssetStoreTools/Images/Characters/maqi_s/maqi_s_Determination.png" },
                    { FacialExpressionType.Scared, "Assets/AssetStoreTools/Images/Characters/maqi_s/maqi_s_Scared.png" },
                    { FacialExpressionType.Despair, "Assets/AssetStoreTools/Images/Characters/maqi_s/maqi_s_Despair.png" },
                })
        },
        {
            2, new CharacterData(2, "ポーラン少年時代", "ポーラン", 
                new Color(0.294f, 0.000f, 0.510f, 1.000f), 0.06f,
                new Dictionary<FacialExpressionType, string>
                {
                    { FacialExpressionType.Default, "Assets/AssetStoreTools/Images/Characters/baolong/baolong_s_Default.png" },
                    { FacialExpressionType.Beaming, "Assets/AssetStoreTools/Images/Characters/baolong/baolong_s_Beaming.png" },
                    { FacialExpressionType.Angry, "Assets/AssetStoreTools/Images/Characters/baolong/baolong_s_Angry.png" },
                    { FacialExpressionType.Worry, "Assets/AssetStoreTools/Images/Characters/baolong/baolong_s_Worry.png" },
                    { FacialExpressionType.Despair, "Assets/AssetStoreTools/Images/Characters/baolong_s/baolong_s_Despair.png" },
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
        if (character.HasExpression(expression))
        {
            return character.GetExpressionPath(expression);
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
