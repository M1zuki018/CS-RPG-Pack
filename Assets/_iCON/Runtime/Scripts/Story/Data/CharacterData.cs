using System.Collections.Generic;
using UnityEngine;

namespace iCON.System
{   
    /// <summary>
    /// キャラクターデータ
    /// </summary>
    public class CharacterData
    {
        /// <summary>
        /// キャラクターID
        /// </summary>
        public int Id;
        
        /// <summary>
        /// フルネーム
        /// </summary>
        public string FullName;
        
        /// <summary>
        /// 表示名
        /// </summary>
        public string DisplayName;
        
        /// <summary>
        /// キャラクターカラー
        /// </summary>
        public Color CharacterColor;

        /// <summary>
        /// 文字送りの速さ
        /// </summary>
        public float TextSpeed;
        
        /// <summary>
        /// 表情差分とファイルパスのkvp
        /// </summary>
        public Dictionary<FacialExpressionType, string> ExpressionPaths;

        public CharacterData(int id, string fullName, string displayName, Color characterColor, 
            float textSpeed, Dictionary<FacialExpressionType, string> expressionPaths)
        {
            Id = id;
            FullName = fullName;
            DisplayName = displayName;
            CharacterColor = characterColor;
            TextSpeed = textSpeed;
            ExpressionPaths = expressionPaths;
        }
    }
}