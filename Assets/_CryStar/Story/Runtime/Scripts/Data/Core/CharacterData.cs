using System.Collections.Generic;
using CryStar.Story.Enums;
using UnityEngine;

namespace CryStar.Story.Data
{   
    /// <summary>
    /// キャラクターデータ
    /// </summary>
    public class CharacterData
    {
        #region Private Fields

        private int _id;
        private string _fullName;
        private string _displayName;
        private Color _characterColor;
        private float _textSpeed;
        private Dictionary<FacialExpressionType, string> _expressionPaths;
        
        #endregion
        
        /// <summary>
        /// キャラクターID
        /// </summary>
        public int Id => _id;
        
        /// <summary>
        /// フルネーム
        /// </summary>
        public string FullName => _fullName; 
        
        /// <summary>
        /// 表示名
        /// </summary>
        public string DisplayName => _displayName;
        
        /// <summary>
        /// キャラクターカラー
        /// </summary>
        public Color CharacterColor => _characterColor;

        /// <summary>
        /// 文字送りの速さ
        /// </summary>
        public float TextSpeed => _textSpeed;
        
        /// <summary>
        /// 表情差分とファイルパスのkvp
        /// </summary>
        public Dictionary<FacialExpressionType, string> ExpressionPaths => _expressionPaths;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterData(int id, string fullName, string displayName, Color characterColor, 
            float textSpeed, Dictionary<FacialExpressionType, string> expressionPaths)
        {
            _id = id;
            _fullName = fullName;
            _displayName = displayName;
            _characterColor = characterColor;
            _textSpeed = textSpeed;
            _expressionPaths = expressionPaths;
        }
        
        /// <summary>
        /// 指定された表情の画像パスを取得
        /// </summary>
        public string GetExpressionPath(FacialExpressionType expressionType)
        {
            return _expressionPaths?.GetValueOrDefault(expressionType);
        }

        /// <summary>
        /// 指定された表情が利用可能かチェック
        /// </summary>
        public bool HasExpression(FacialExpressionType expressionType)
        {
            return _expressionPaths?.ContainsKey(expressionType) == true;
        }
        
        /// <summary>
        /// 文字列表現を取得
        /// </summary>
        public override string ToString()
        {
            return $"Character[{_id}]: {_displayName} ({_fullName})";
        }
    }
}